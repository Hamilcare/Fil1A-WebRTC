function init(){

    renderer = new THREE.WebGLRenderer();

    renderer.setSize( window.innerWidth, window.innerHeight );
    document.getElementById('container').appendChild(renderer.domElement);

    scene = new THREE.Scene();

    camera = new THREE.PerspectiveCamera(50, window.innerWidth / window.innerHeight, 0.01, 10000 );
    //camera.position.set(0, 0, -3);
    //camera.position.set(0, 0, -1000);
	camera.position.set(0, 0, -5);
    camera.lookAt(scene.position);

    scene.add(camera);

	var material = new THREE.MeshBasicMaterial( { color: 0xff0000, wireframe: true } );

    renderer.render( scene, camera );
}


var currentTimestamp; //currentTimestamp
var cloudDataBuffer = "";  //current data buffer
var timestampLength
//Store cloud data till timestamp is the same
function handleCloudData(data){
	//console.log(data);
	if(!timestampLength){
		timestampLength = data.indexOf(';');
	}
	//console.log(timestampLength);
	trameTimestamp = Number(data.substring(0,timestampLength));
	
	if(trameTimestamp == currentTimestamp){
		cloudDataBuffer += data.substring(timestampLength+2);
	}
	else{
		
		synchronousCloud(cloudDataBuffer);
		cloudDataBuffer = data.substring(11);
		currentTimestamp = trameTimestamp;
	}
}

function renderUsingOneBufferGeometry(){
	console.log("Starting rendering cubes using one BufferGeometry");
	let startMethod, endMethod;
	startMethod = performance.now();
	//var material = new THREE.MeshBasicMaterial( { color: 0xff0000 } );
	/*var material = new THREE.PointsMaterial({
		size:10,
		transparent: true,
		opacity: 0.9,
	vertexColors: THREE.VertexColors});*/
	for (j=0; j< numberOfRendering; j++){
		myScene = new THREE.Scene();
		var coordinates =  generateArrayOfCoordinates();
		var myColors = generateArrayOfColors();
		console.log(myColors);
		console.log("number of coordinates: "+(coordinates.length/3));
		var geometry = new THREE.BufferGeometry();
		//here 3 means 3 values per coordinate
		geometry.addAttribute('position', new THREE.BufferAttribute(coordinates, 3));
		geometry.colors = myColors;
		//geometry.addAttribute( 'color', new THREE.BufferAttribute(colors, 3 ) );
		//var mesh = new THREE.Mesh(geometry,material);
		var cloudPoints = new THREE.Points(geometry);
		//myScene.add(mesh);
		cloudPoints.geometry.colorsNeedUpdate=true;
		myScene.add(cloudPoints);
		renderer.render(myScene,camera);
	}
	
	endMethod = performance.now();
	console.log("method took " + (endMethod - startMethod)+" ms");
}

function parseColorFromData(){
	return true;
}

function parseCoordinatesFromData(data){
	var arrayOfCoordinates = data.split(";");
	var array = new Float32Array(arrayOfCoordinates.length/2);
	var index = 0;
	for(let i=0; i< arrayOfCoordinates.length; i=i+6){
		array[index] = parseFloat(arrayOfCoordinates[i]);
		index++;
		array[index] = parseFloat(arrayOfCoordinates[i+1]);
		index++;
		array[index] = parseFloat(arrayOfCoordinates[i+2]);
		index++;
	}
	
	return array;
}


function synchronousCloud(data){
	var material = new THREE.PointsMaterial({
		size:0.1,
		transparent: true,
		opacity: 0.9,
		color: 0xff0000});
	let startMethod, endMethod, parsedData;
	startMethod = performance.now();
	geometry = new THREE.BufferGeometry();//Globale pour le moment pour le debug
	//On parse d'abord les coordinates
	var promiseParseCoordinate = new Promise(function(resolve,reject){
		parsedData = parseCoordinatesFromData(data)
		resolve(parsedData);
		console.log("J'ai parse les coord");
		return parsedData;
	});
	//On parse ensuite les couleurs
	promiseParseCoordinate.then(function(parsedData){
		parsedColors = parseColorFromData(data);
		console.log("J'ai parse les colors");
		return parsedColors;
	})	
	//On ajoute les points et les couleurs à la geometry 
	.then(function(parsedColors){
		console.log(parsedData);
		console.log(parsedColors);
		geometry.addAttribute('position', new THREE.BufferAttribute(parsedData, 3));//debug
		//@TODO PTDR ON AJOUTE LES COULEURS
		cloudPoints = new THREE.Points(geometry,material);
		return cloudPoints;
	})
	//On refresh la scene puis on ajoute notre cloud
	.then(function(cloudPoints){
		myScene = new THREE.Scene();
		myScene.add(cloudPoints);
		renderer.render( myScene, camera );
		endMethod = performance.now();
		console.log("Method took "+(endMethod - startMethod)+"ms");
	});
}


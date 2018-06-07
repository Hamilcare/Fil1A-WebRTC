

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
//var arrayOfDataReadyToRender = []; //array of data ready to be process by rendering method
var isRenderingReady = true; //true if rendering method is ready to process a new set of data
var timestampLength;

//Store cloud data while timestamp is the same
function handleCloudData(data){
	
	console.log(data);
	//console.log(arrayOfDataReadyToRender.length);
	if(!timestampLength){
		timestampLength = data.indexOf(';');
	}
	//console.log(timestampLength);
	trameTimestamp = Number(data.substring(0,timestampLength));
	if(!currentTimestamp)//sur la première trame reçue
		currentTimestamp=trameTimestamp;
	
	console.log("currentTimestamp");
	console.log(currentTimestamp);
	
	console.log("trameTimestamp");
	console.log(trameTimestamp);
	if(trameTimestamp == currentTimestamp){
		cloudDataBuffer += data.substring(timestampLength+2);
	}
	else{
		synchronousCloud(cloudDataBuffer);
		//arrayOfDataReadyToRender.push(cloudDataBuffer);
		cloudDataBuffer = data.substring(timestampLength+2);
		currentTimestamp = trameTimestamp;
	}
	
	//if(isRenderingReady && arrayOfDataReadyToRender.length>0){
	//	synchronousCloud(arrayOfDataReadyToRender.shift());
	//}
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
		//console.log(myColors);
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

function parseColorFromData(data){
	var arrayOfCoordinates = data.split(";");
	while((arrayOfCoordinates.length)%6 !=0){
		arrayOfCoordinates.push("0");
	}
	var array = new Float32Array(arrayOfCoordinates.length/2);
	var index = 0;
	for(let i=0; i< arrayOfCoordinates.length-6; i=i+6){
		array[index] = parseFloat(arrayOfCoordinates[i+3])/255;//R
		index++;
		array[index] = parseFloat(arrayOfCoordinates[i+4])/255;//G
		index++;
		array[index] = parseFloat(arrayOfCoordinates[i+5])/255;//B
		index++;
	}
	return array;
}

function parseCoordinatesFromData(data){
	var arrayOfCoordinates = data.split(";");
	while((arrayOfCoordinates.length)%6 !=0){
		arrayOfCoordinates.push("0");
	}
	var array = new Float32Array(arrayOfCoordinates.length/2);
	var index = 0;
	for(let i=0; i< arrayOfCoordinates.length-6; i=i+6){
		//X
		array[index] = parseFloat(arrayOfCoordinates[i]) || 0;
		if(isNaN(array[index])){
			console.log("nan detected"+index);
			array[index] = 0;
		}
		index++;
		//Y
		array[index] = parseFloat(arrayOfCoordinates[i+1])|| 0;
		if(isNaN(array[index])){
			console.log("nan detected"+index);
			array[index] = 0;
		}
		index++;
		//Z
		array[index] = parseFloat(arrayOfCoordinates[i+2])|| 0;
		if(isNaN(array[index])){
			console.log("nan detected "+index);
			array[index] = 0;
		}
		index++;
	}
	
	return array;
}


function synchronousCloud(data){
	console.log("Starting rendering cubes using one synchronousCloud");
	//Starting new rendering so not ready to process another set of data
	isRenderingReady = false;
	var material = new THREE.PointsMaterial({
		size:0.05,
		transparent: true,
		opacity: 0.9,
		color: 'white',
		vertexColors: THREE.VertexColors});
	let startMethod, endMethod, parsedData;
	startMethod = performance.now();
	geometry = new THREE.BufferGeometry();//Globale pour le moment pour le debug
	//On parse d'abord les coordinates
	var promiseParseCoordinate = new Promise(function(resolve,reject){
		parsedData = parseCoordinatesFromData(data)
		resolve(parsedData);
		//console.log("J'ai parse les coord");
		return parsedData;
	});
	//On parse ensuite les couleurs
	promiseParseCoordinate.then(function(parsedData){
		parsedColors = parseColorFromData(data);
		//console.log("J'ai parse les colors");
		return parsedColors;
	})	
	//On ajoute les points et les couleurs à la geometry 
	.then(function(parsedColors){
		
		console.log("Colors:");
		console.log(parsedColors);
		geometry.addAttribute('position', new THREE.BufferAttribute(parsedData, 3));//debug
		geometry.addAttribute('color', new THREE.BufferAttribute(parsedColors, 3));//debug
		
		cloudPoints = new THREE.Points(geometry,material);
		//cloudPoints = new THREE.Mesh(geometry);
		return cloudPoints;
	})
	//On refresh la scene puis on ajoute notre cloud
	.then(function(cloudPoints){
		myScene = new THREE.Scene();
		myScene.add(cloudPoints);
		renderer.render( myScene, camera );
		endMethod = performance.now();
		console.log("Method took "+(endMethod - startMethod)+"ms");
		
		//Rendering is finished, method is ready to process new set of data
		isRenderingReady = true;
	});
}

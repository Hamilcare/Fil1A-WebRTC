var renderer, scene, camera, mesh;

let arrayOfSkeletonPoint = [];
let arrayOfCloudPoint = [];

function init(){
    // on initialise le moteur de rendu
    renderer = new THREE.WebGLRenderer();

    // si WebGL ne fonctionne pas sur votre navigateur vous pouvez utiliser le moteur de rendu Canvas à la place
    // renderer = new THREE.CanvasRenderer();
    renderer.setSize( window.innerWidth, window.innerHeight );
    document.getElementById('container').appendChild(renderer.domElement);

    // on initialise la scène
    scene = new THREE.Scene();

    // on initialise la camera que l’on place ensuite sur la scène
    //camera = new THREE.PerspectiveCamera(50, window.innerWidth / window.innerHeight, 1, 10000 );
    camera = new THREE.PerspectiveCamera(50, window.innerWidth / window.innerHeight, 0.01, 10000 );
    //camera.position.set(0, 0, 1000);
    camera.position.set(0, 0, -3);
    camera.lookAt(scene.position);

    scene.add(camera);

	var material = new THREE.MeshBasicMaterial( { color: 0xff0000, wireframe: true } );
/*
    // on créé un  cube au quel on définie un matériau puis on l’ajoute à la scène
    var geometry = new THREE.CubeGeometry( 200, 200, 200 );
    var material = new THREE.MeshBasicMaterial( { color: 0xff0000, wireframe: true } );
    mesh = new THREE.Mesh( geometry, material );
    scene.add( mesh );


	var geometry = new THREE.Geometry();

	geometry.vertices.push(
		new THREE.Vector3( -10,  -10, 0 ),
		new THREE.Vector3(-10,10, 0),
		new THREE.Vector3( 10, 10, 0 ),
		new THREE.Vector3(  10, -10, 0 )

	);*/

	//geometry.faces.push( new THREE.Face3( 0, 1, 2 ) );

	//geometry.computeBoundingSphere();
//	scene.add(new THREE.Line(geometry, material));
//	scene.add(new THREE.Mesh(geometry, material));

    // on effectue le rendu de la scène
    renderer.render( scene, camera );
}

function handleReceivedSkeletonCoordinates(data){
  //removeSkeletonFromScene();
	//console.log("reveive data: "+data);

	arrayOfCoordinates = data.split(";");
	//console.log(arrayOfCoordinates);

	let arrayOfVector = [];
	let arrayOfGeometry = [];
  scene = new THREE.Scene();
	for(let i=0; i< arrayOfCoordinates.length; i=i+6){
    //console.log('traitement du point n°'+i/6);
		arrayOfGeometry[i] = new THREE.CubeGeometry(0.1, 0.1, 0.1);//Le cube représente l'articulation
		arrayOfGeometry[i].position = new THREE.Vector3(arrayOfCoordinates[i],arrayOfCoordinates[i+1],arrayOfCoordinates[i+2]);
		var material = new THREE.MeshBasicMaterial( { color: 0xffffff, wireframe: true } );
		//material.color.setRGB(arrayOfCoordinates[i+3],arrayOfCoordinates[i+4],arrayOfCoordinates[i+5]);
    material.color.setRGB(255,255,255);
		mesh = new THREE.Mesh(arrayOfGeometry[i], material);
    mesh.drawMode = THREE.TriangleStripDrawMode;//Peut être plus économique
    mesh.name="skeleton"+i/6;
		scene.add(mesh);
		mesh.position.set(arrayOfCoordinates[i],arrayOfCoordinates[i+1],arrayOfCoordinates[i+2]);
    arrayOfSkeletonPoint[i/6] = mesh;
    //console.log(scene);
	}
	//	//console.log(arrayOfVector);
	//console.log(arrayOfGeometry);
	////console.log(scene);
	renderer.render( scene, camera );
}

function handleReceivedCloudCoordinates(data){
  //removeSkeletonFromScene();
	//console.log("reveive data: "+data);

	arrayOfCoordinates = data.split(";");
	//console.log(arrayOfCoordinates);
  //removeCloudFromScene();
	let arrayOfVector = [];
	let arrayOfGeometry = [];
  //scene = new THREE.Scene();
	for(let i=0; i< arrayOfCoordinates.length; i=i+60){
    //console.log('traitement du point n°'+i/6);
		arrayOfGeometry[i] = new THREE.CubeGeometry(0.05, 0.05, 0.05);//Le cube représente l'articulation
		arrayOfGeometry[i].position = new THREE.Vector3(arrayOfCoordinates[i],arrayOfCoordinates[i+1],arrayOfCoordinates[i+2]);
		var material = new THREE.MeshBasicMaterial( { color: 0xffffff, wireframe: true } );
		//material.color.setRGB(arrayOfCoordinates[i+3],arrayOfCoordinates[i+4],arrayOfCoordinates[i+5]);
    material.color.setRGB(255,0,0);
		mesh = new THREE.Mesh(arrayOfGeometry[i], material);
    mesh.drawMode = THREE.TriangleStripDrawMode;//Peut être plus économique
    mesh.name="skeleton"+i/6;
		scene.add(mesh);
		mesh.position.set(arrayOfCoordinates[i],arrayOfCoordinates[i+1],arrayOfCoordinates[i+2]);
    arrayOfCloudPoint[i/6] = mesh;
    //console.log(scene);
	}
	//	//console.log(arrayOfVector);
	//console.log(arrayOfGeometry);
	////console.log(scene);
	renderer.render( scene, camera );
}

function removeSkeletonFromScene(){
  //console.log("Start removing skeleton");
  //console.log(scene);
  for(i=0; i<arrayOfSkeletonPoint.length; i++){
    scene.remove(arrayOfSkeletonPoint[i]);//@TODO dispose geometry on trigger
    arrayOfSkeletonPoint[i].geometry.dispose();
  }
  //console.log(scene);
  //renderer.render( scene, camera );
}

function removeCloudFromScene(){
  //console.log("Start removing skeleton");
  //console.log(scene);
  for(i=0; i<arrayOfCloudPoint.length; i++){
    scene.remove(arrayOfCloudPoint[i]);//@TODO dispose geometry on trigger
    arrayOfCloudPoint[i].geometry.dispose();
  }
  //console.log(scene);
  //renderer.render( scene, camera );
}

function animate(){
    requestAnimationFrame( animate );
    mesh.rotation.x += 0.01;
    mesh.rotation.y += 0.02;
    renderer.render( scene, camera );
}

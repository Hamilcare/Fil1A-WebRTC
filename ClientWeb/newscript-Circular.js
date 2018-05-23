var addButton = document.getElementById('addButton');
var numberOfSimulatenousClouds = 1;
var numberOfSimulatenousSkeletons = 1;
var CloudNumberPointer = 0;
var SkeletonNumberPointer = 0;
var moveUp = 600;
var moveBack = 1000;
var moveleft = -1200;
var mouse;
//Material du nuage de points
var material = new THREE.PointsMaterial( { size: 10, vertexColors: THREE.VertexColors  , opacity : 0.7} );
var materialSkeleton = new THREE.LineBasicMaterial( { color: 0xffffff, linewidth: 10} );
var indicePairPointSkeleton = [3,2,2,4,2,8,2,20,4,5,5,6,6,7,7,21,7,22,8,9,9,10,10,11,11,24,11,23,20,1,1,0,0,12,0,16,12,13,13,14,14,15,16,17,17,18,18,19];
var initMemory = true;
var initMemorySkeleton = true;
var myCloud = [];
var mySkeleton = [];
var particles2 = [];
var particlesSkeleton = [];
var reductionFactor = 0.75;
var scene, camera, renderer;
var container, HEIGHT,
    WIDTH, fieldOfView, aspectRatio,
    nearPlane, farPlane, stats,
    geometry, particleCount,
    i, h, color, size,
    materials = [],
    mouseX = 0,
    mouseY = 0,
    windowHalfX, windowHalfY, cameraZ,
    fogHex, fogDensity, parameters = {},
    particles;
var startNewCloud = true;
var startNewSkeleton = true;
var j = 0;
init();
animate();


addButton.onclick = function(){
    for (var i = 0; i<1; i++){
        var pointToCreate = '1721 {"joints":[{"Key":0,"Value":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":1,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}},{"Key":2,"Value\":{"x":'+ (Math.random()-0.5)*10 +',"y":'+ (Math.random()-0.5)*10 +',"z":'+ (Math.random()-0.5)*10 +'}}],"tag":72057594037928249}';
        generatePoint(pointToCreate);
    }
};

function generatePoint(pointTab){
    if(CloudNumberPointer>numberOfSimulatenousClouds){
        CloudNumberPointer = 0;
        initMemory = false;
    }
    if(startNewCloud){
        myCloud[CloudNumberPointer] = new THREE.Geometry();
        startNewCloud = false;
    }

    for(var i = 1; i<pointTab.length-7; i+=6){ //TODO virer le 7 ?
        //Instantation du vertex(Point)
    var vertex = new THREE.Vector3();
    
    vertex.x = pointTab[i]/reductionFactor+moveleft;
    vertex.y = moveUp/reductionFactor-pointTab[i+1]/reductionFactor;
    vertex.z = moveBack/reductionFactor-pointTab[i+2]/reductionFactor;
      
      //Envoie des vertex dans la Geometry
      myCloud[CloudNumberPointer].vertices.push(vertex);
    
    //Instantation de la couleur en RGB 
        var color = new THREE.Color("rgb("+pointTab[i+3]+","+pointTab[i+4]+","+pointTab[i+5]+")");

    //Envoie des couleurs dans la Geometry
        myCloud[CloudNumberPointer].colors.push(color);
    }

  //Affichage du nuage de points si au moins 6000 points dans le nuage
    if(myCloud[CloudNumberPointer].vertices.length>6000){
        startNewCloud = true;
        if(!initMemory){
            var selectedObject = scene.getObjectByName(particles2[CloudNumberPointer].name);
            scene.remove( selectedObject );

        }
    //Création du nuage de points avec la Geometry(Positions + couleurs) et le Material(Texture...)
        particles2[CloudNumberPointer] = new THREE.Points(myCloud[CloudNumberPointer], material);
        particles2[CloudNumberPointer].name = "myParticules"+CloudNumberPointer;
        scene.add(particles2[CloudNumberPointer]);
        CloudNumberPointer++;
    }


}

function SkeletonPoint2Vector(point)
{
  var vector = new THREE.Vector3();
/*  vector.x = point.Value.x/reductionFactor+moveleft;
  vector.y = moveUp/reductionFactor-point.Value.y/reductionFactor;
  vector.z = moveBack/reductionFactor-point.Value.z/reductionFactor;*/
  vector.x = point.Value.x*350;
  vector.y = point.Value.y*350;
  vector.z = point.Value.z*350;
  return vector;
}

function generateSkeleton(skeleton){
  if(SkeletonNumberPointer>numberOfSimulatenousSkeletons){
      SkeletonNumberPointer = 0;
      initMemorySkeleton = false;
  }
  if(startNewSkeleton){
      startNewSkeleton = false;
      mySkeleton[SkeletonNumberPointer] = new THREE.Geometry();
  }

  for(var i=0; i<indicePairPointSkeleton.length ; i++)
  {
    mySkeleton[SkeletonNumberPointer].vertices.push(SkeletonPoint2Vector(skeleton[indicePairPointSkeleton[i]]));
  }

    //Affichage du squelette si 25 joints
    if(skeleton.length==25){
        startNewSkeleton = true;
        if(!initMemorySkeleton){
            var selectedObject = scene.getObjectByName(particlesSkeleton[SkeletonNumberPointer].name);
            scene.remove( selectedObject );
        }
    
        particlesSkeleton[SkeletonNumberPointer] = new THREE.LineSegments(mySkeleton[SkeletonNumberPointer], materialSkeleton);
        particlesSkeleton[SkeletonNumberPointer].name = "mySkeleton"+SkeletonNumberPointer;
        scene.add(particlesSkeleton[SkeletonNumberPointer]);
        SkeletonNumberPointer++;
    }


}

    function init() {
        main();
        HEIGHT = window.innerHeight;
        WIDTH = window.innerWidth;
        windowHalfX = WIDTH / 2;
        windowHalfY = HEIGHT / 2;

        fieldOfView = 75;
        aspectRatio = WIDTH / HEIGHT;
        nearPlane = 1;
        farPlane = 3000;

        mouse = new THREE.Vector3( 0, 0, 1 );
        document.addEventListener( 'mousemove', onDocumentMouseMove, false );

        cameraZ = farPlane / 3;
        fogHex = 0x000000;
        fogDensity = 0.0007;

        camera = new THREE.PerspectiveCamera(fieldOfView, aspectRatio, nearPlane, farPlane);
        camera.position.z = cameraZ;

        scene = new THREE.Scene();
        scene.fog = new THREE.FogExp2(fogHex, fogDensity);

        container = document.createElement('div');
        document.body.appendChild(container);
        document.body.style.margin = 0;
        document.body.style.overflow = 'hidden';

        geometry = new THREE.Geometry();

        particleCount = 20;

        renderer = new THREE.WebGLRenderer();
        renderer.setPixelRatio(window.devicePixelRatio);
        renderer.setSize(WIDTH, HEIGHT);

        container.appendChild(renderer.domElement);

        window.addEventListener('resize', onWindowResize, false);

    }

    function animate() {
        requestAnimationFrame(animate);
        render();
    }

    function render() {

        //camera.position.x += ( mouse.x - camera.position.x ) * 0.05;
        //camera.position.y += ( - mouse.y - camera.position.y ) * 0.05;
        //camera.lookAt(new THREE.Vector3());

        renderer.render(scene, camera);
    }

    function onWindowResize() {
        windowHalfX = window.innerWidth / 2;
        windowHalfY = window.innerHeight / 2;

        camera.aspect = window.innerWidth / window.innerHeight;
        camera.updateProjectionMatrix();
        renderer.setSize(window.innerWidth, window.innerHeight);
    }

    function onDocumentMouseMove( event ) {
        mouse.x = ( event.clientX - window.innerWidth / 2 ) * 8;
        mouse.y = ( event.clientY - window.innerHeight / 2 ) * 8;
      }

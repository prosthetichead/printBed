import * as THREE from 'three';
import { STLLoader } from 'three/addons/loaders/STLLoader.js';
import { OrbitControls } from 'three/addons/controls/OrbitControls.js';

//let container, stats;

//let camera, cameraTarget, scene, renderer; 

init();

function getRandomColor() {
    var letters = '0123456789ABCDEF';
    var color = '#';
    for (var i = 0; i < 6; i++) {
        color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
}

function init() {
    //loop though all divs with data-modelviewer="true"
    const elems = document.querySelectorAll('[data-modelviewer="true"]');
    for (const elem of elems) {
        SetupSTL(elem);
    }
}

function SetupSTL(elem) {
    const modelPath = elem.getAttribute('data-modelviewer-path');
    var camera = new THREE.PerspectiveCamera(70, elem.clientWidth / elem.clientHeight, 1, 1000);
    var renderer = new THREE.WebGLRenderer({ antialias: true, alpha: true });

    //renderer.outputEncoding = THREE.sRGBEncoding;
    renderer.setSize(elem.clientWidth, elem.clientHeight);

    elem.appendChild(renderer.domElement);

    window.addEventListener('resize', function () {
        renderer.setSize(elem.clientWidth, elem.clientHeight);
        camera.aspect = elem.clientWidth / elem.clientHeight;
        camera.updateProjectionMatrix();
    }, false);

    var controls = new OrbitControls(camera, renderer.domElement);
    controls.enableDamping = true;
    controls.rotateSpeed = 1;
    controls.dampingFactor = 0.1;
    controls.enableZoom = true;
    controls.autoRotate = true;
    controls.autoRotateSpeed = .85;

    var scene = new THREE.Scene();

    //scene.add(new THREE.AmbientLight(0xFFFFFF, 1));
    //const light1 = new THREE.HemisphereLight(0xffffff, 0x080820, 1);
    //const light2 = new THREE.DirectionalLight(0xffffff, 100);
    //light1.castShadow = true;
    //scene.add(light1);
    //camera.add(light2);
    


    const loader = new STLLoader();
    var color = getRandomColor();
    new STLLoader().load(modelPath, function (geometry) {
        const material = new THREE.MeshNormalMaterial()
        //var material = new THREE.MeshBasicMaterial({
        //    color: '#FF6219',
        //    flatShading: false,
        //    roughness: 1,
        //    metalness: 0
        //});
        var mesh = new THREE.Mesh(geometry, material);
        scene.add(mesh);
        var middle = new THREE.Vector3();
        geometry.computeBoundingBox();
        geometry.boundingBox.getCenter(middle);

        mesh.geometry.applyMatrix4(new THREE.Matrix4().makeTranslation(
            -middle.x, -middle.y, -middle.z));
        var largestDimension = Math.max(geometry.boundingBox.max.x,
            geometry.boundingBox.max.y,
            geometry.boundingBox.max.z)
        camera.position.z = largestDimension * 1.5;

        var animate = function () {
            requestAnimationFrame(animate);
            controls.update();
            renderer.render(scene, camera);
        };
        animate();
    });
}
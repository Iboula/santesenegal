// SanteSenegal 3D Visualization — Three.js
// Chargé dans Blazor WASM, Three.js est global depuis le CDN

const THREE = window.THREE;

let scene, camera, renderer, controls;
let accidentGroup, hopitalGroup;
let animationId;

const ACCIDENT_COLORS = {
    critique: 0xe63946,
    eleve: 0xff9f1c,
    moyen: 0xffd700,
    faible: 0x2ec4b6
};

const HOPITAL_COLOR = 0x3a86ff;

export function initThreeJs(canvasId) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) {
        console.error('Canvas not found:', canvasId);
        return;
    }

    scene = new THREE.Scene();
    scene.background = new THREE.Color(0x1a1a2e);
    scene.fog = new THREE.Fog(0x1a1a2e, 50, 200);

    camera = new THREE.PerspectiveCamera(60, canvas.clientWidth / canvas.clientHeight, 0.1, 1000);
    camera.position.set(0, 40, 60);
    camera.lookAt(0, 0, 0);

    renderer = new THREE.WebGLRenderer({ canvas: canvas, antialias: true });
    renderer.setSize(canvas.clientWidth, canvas.clientHeight);
    renderer.shadowMap.enabled = true;

    const ambientLight = new THREE.AmbientLight(0xffffff, 0.6);
    scene.add(ambientLight);

    const dirLight = new THREE.DirectionalLight(0xffffff, 0.8);
    dirLight.position.set(50, 100, 50);
    dirLight.castShadow = true;
    scene.add(dirLight);

    const terrainGeometry = new THREE.PlaneGeometry(200, 150, 50, 50);
    const terrainMaterial = new THREE.MeshStandardMaterial({
        color: 0x2d4a3e,
        roughness: 0.9,
        metalness: 0.1,
        flatShading: true
    });
    const posAttribute = terrainGeometry.attributes.position;
    for (let i = 0; i < posAttribute.count; i++) {
        const x = posAttribute.getX(i);
        const y = posAttribute.getY(i);
        const z = Math.sin(x * 0.1) * Math.cos(y * 0.1) * 2;
        posAttribute.setZ(i, z);
    }
    terrainGeometry.computeVertexNormals();

    const terrain = new THREE.Mesh(terrainGeometry, terrainMaterial);
    terrain.rotation.x = -Math.PI / 2;
    terrain.receiveShadow = true;
    scene.add(terrain);

    const grid = new THREE.GridHelper(200, 40, 0x444444, 0x222222);
    scene.add(grid);

    accidentGroup = new THREE.Group();
    hopitalGroup = new THREE.Group();
    scene.add(accidentGroup);
    scene.add(hopitalGroup);

    if (window.THREE.OrbitControls) {
        controls = new THREE.OrbitControls(camera, renderer.domElement);
        controls.enableDamping = true;
        controls.dampingFactor = 0.05;
    }

    animate();
    window.addEventListener('resize', onWindowResize);
}

function animate() {
    animationId = requestAnimationFrame(animate);
    if (controls) controls.update();
    renderer.render(scene, camera);
}

function onWindowResize() {
    if (!camera || !renderer) return;
    const canvas = renderer.domElement;
    camera.aspect = canvas.clientWidth / canvas.clientHeight;
    camera.updateProjectionMatrix();
    renderer.setSize(canvas.clientWidth, canvas.clientHeight);
}

export function clearAccidents() {
    while (accidentGroup.children.length > 0) {
        accidentGroup.remove(accidentGroup.children[0]);
    }
}

export function clearHopitaux() {
    while (hopitalGroup.children.length > 0) {
        hopitalGroup.remove(hopitalGroup.children[0]);
    }
}

export function addAccident(lat, lng, gravite, victimes, description) {
    const color = ACCIDENT_COLORS[gravite] || ACCIDENT_COLORS.faible;
    const height = Math.max(1, (victimes || 1) * 0.8);
    const radius = 0.5;

    const geometry = new THREE.CylinderGeometry(radius, radius, height, 16);
    const material = new THREE.MeshStandardMaterial({
        color: color,
        emissive: color,
        emissiveIntensity: 0.3,
        roughness: 0.4
    });
    const mesh = new THREE.Mesh(geometry, material);
    mesh.position.set(lng * 10, height / 2, -lat * 10);
    mesh.castShadow = true;

    const ringGeo = new THREE.RingGeometry(radius + 0.2, radius + 0.5, 32);
    const ringMat = new THREE.MeshBasicMaterial({
        color: color,
        transparent: true,
        opacity: 0.4,
        side: THREE.DoubleSide
    });
    const ring = new THREE.Mesh(ringGeo, ringMat);
    ring.rotation.x = -Math.PI / 2;
    ring.position.set(lng * 10, 0.05, -lat * 10);

    accidentGroup.add(mesh);
    accidentGroup.add(ring);
}

export function addHopital(lat, lng, nom, lits, adresse) {
    const size = 1.5;
    const height = Math.max(1, (lits || 10) * 0.05 + 1);

    const geometry = new THREE.BoxGeometry(size, height, size);
    const material = new THREE.MeshStandardMaterial({
        color: HOPITAL_COLOR,
        roughness: 0.3,
        metalness: 0.2
    });
    const mesh = new THREE.Mesh(geometry, material);
    mesh.position.set(lng * 10, height / 2, -lat * 10);
    mesh.castShadow = true;

    const crossGeo = new THREE.BoxGeometry(0.8, 0.1, 0.2);
    const crossMat = new THREE.MeshBasicMaterial({ color: 0xffffff });
    const cross1 = new THREE.Mesh(crossGeo, crossMat);
    cross1.position.set(lng * 10, height + 0.05, -lat * 10);
    const cross2 = cross1.clone();
    cross2.rotation.y = Math.PI / 2;

    hopitalGroup.add(mesh);
    hopitalGroup.add(cross1);
    hopitalGroup.add(cross2);
}

export function setCameraPosition(x, y, z) {
    if (camera) {
        camera.position.set(x, y, z);
        camera.lookAt(0, 0, 0);
    }
}

export function dispose() {
    if (animationId) cancelAnimationFrame(animationId);
    window.removeEventListener('resize', onWindowResize);
    if (renderer) renderer.dispose();
    scene = null;
    camera = null;
    renderer = null;
}

// Fonctions helpers pour Blazor (JSON strings)
window.loadAccidents3D = function(jsonStr) {
    const accidents = JSON.parse(jsonStr);
    if (window.clearAccidents) window.clearAccidents();
    for (const acc of accidents) {
        const gravite = acc.nombreDeces > 0 || acc.risqueIncendie ? 'critique'
            : acc.nombreBlessesGraves > 0 ? 'eleve'
            : acc.nombreVictimesEstime >= 3 ? 'moyen' : 'faible';
        if (window.addAccident) window.addAccident(acc.latitude, acc.longitude, gravite, acc.nombreVictimesEstime, acc.description);
    }
};

window.loadHopitaux3D = function(jsonStr) {
    const hopitaux = JSON.parse(jsonStr);
    if (window.clearHopitaux) window.clearHopitaux();
    for (const h of hopitaux) {
        if (window.addHopital) window.addHopital(h.latitude, h.longitude, h.nom, h.capacite, h.adresse);
    }
};

// Expose pour Blazor IJSRuntime
window.initThreeJs = initThreeJs;
window.clearAccidents = clearAccidents;
window.clearHopitaux = clearHopitaux;
window.addAccident = addAccident;
window.addHopital = addHopital;
window.disposeThreeJs = dispose;

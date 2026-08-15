extends Node3D

@onready var camera = $Camera3D
@onready var accident_container = $Accidents
@onready var hopital_container = $Hopitaux
@onready var data_bridge = $DataBridge

# Préchargement des scènes
var scene_accident = preload("res://scenes/marqueur_accident.tscn")
var scene_hopital = preload("res://scenes/marqueur_hopital.tscn")

# Matériaux pour les différentes gravités
var mat_critique = StandardMaterial3D.new()
var mat_eleve = StandardMaterial3D.new()
var mat_moyen = StandardMaterial3D.new()
var mat_faible = StandardMaterial3D.new()

func _ready():
	_setup_materials()
	_setup_camera()
	# Signal prêt pour JavaScript
	if OS.has_feature("web"):
		JavaScriptBridge.eval("window.godotReady = true")

func _setup_materials():
	mat_critique.albedo_color = Color(0.9, 0.15, 0.15)
	mat_critique.emission_enabled = true
	mat_critique.emission = Color(0.9, 0.15, 0.15)
	mat_critique.emission_energy_multiplier = 0.5

	mat_eleve.albedo_color = Color(1.0, 0.6, 0.1)
	mat_eleve.emission_enabled = true
	mat_eleve.emission = Color(1.0, 0.6, 0.1)
	mat_eleve.emission_energy_multiplier = 0.4

	mat_moyen.albedo_color = Color(1.0, 0.85, 0.0)
	mat_moyen.emission_enabled = true
	mat_moyen.emission = Color(1.0, 0.85, 0.0)
	mat_moyen.emission_energy_multiplier = 0.3

	mat_faible.albedo_color = Color(0.2, 0.8, 0.7)
	mat_faible.emission_enabled = true
	mat_faible.emission = Color(0.2, 0.8, 0.7)
	mat_faible.emission_energy_multiplier = 0.2

func _setup_camera():
	camera.position = Vector3(0, 40, 60)
	camera.look_at(Vector3.ZERO)

func clear_accidents():
	for child in accident_container.get_children():
		child.queue_free()

func clear_hopitaux():
	for child in hopital_container.get_children():
		child.queue_free()

func add_accident(lat: float, lng: float, gravite: String, victimes: int):
	var instance = scene_accident.instantiate()
	var cylindre = instance.get_node("Cylindre")
	var anneau = instance.get_node("Anneau")

	var height = clampf(float(victimes) * 0.8, 0.8, 5.0)
	cylindre.mesh.height = height
	instance.position = Vector3(lng * 10, height / 2, -lat * 10)

	var mat = mat_faible
	match gravite:
		"critique": mat = mat_critique
		"eleve": mat = mat_eleve
		"moyen": mat = mat_moyen

	cylindre.material_override = mat
	anneau.material_override = mat
	accident_container.add_child(instance)

func add_hopital(lat: float, lng: float, nom: String, lits: int):
	var instance = scene_hopital.instantiate()
	var height = clampf(float(lits) * 0.05 + 1.0, 1.0, 4.0)
	var batiment = instance.get_node("Batiment")
	batiment.mesh.size.y = height
	instance.position = Vector3(lng * 10, height / 2, -lat * 10)
	hopital_container.add_child(instance)

# Appelé depuis JavaScript via le DataBridge
func _on_js_data_received(data: String):
	var json = JSON.parse_string(data)
	if json == null:
		return

	if json.has("accidents"):
		clear_accidents()
		for acc in json["accidents"]:
			add_accident(acc["lat"], acc["lng"], acc["gravite"], acc["victimes"])

	if json.has("hopitaux"):
		clear_hopitaux()
		for h in json["hopitaux"]:
			add_hopital(h["lat"], h["lng"], h["nom"], h["lits"])

extends Node

# Pont de données entre JavaScript et Godot
# Expose des fonctions callable depuis le navigateur

@onready var carte = get_parent()

func _ready():
	# Enregistre les callbacks JS
	if OS.has_feature("web"):
		JavaScriptBridge.eval("""
			window.sendDataToGodot = function(jsonStr) {
				if (window.godotInstance) {
					window.godotInstance.call('DataBridge', '_on_js_data_received', jsonStr);
				}
			};
		""")

func _on_js_data_received(data: String):
	if carte and carte.has_method("_on_js_data_received"):
		carte._on_js_data_received(data)

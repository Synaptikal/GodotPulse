@tool
extends EditorPlugin

const TOGGLE_ACTION := "godot_pulse_toggle"

func _enter_tree() -> void:
	# Register input action for toggling overlay
	_register_input_action()

	# Check if we have .NET support before registering the C# autoload
	if OS.has_feature("dotnet"):
		# Dynamically construct autoload path based on plugin location
		var plugin_dir = get_script().get_path().get_basename().get_basename()
		var autoload_path = plugin_dir + "/autoload/GodotPulse.cs"
		add_autoload_singleton("GodotPulse", autoload_path)
	else:
		printerr("GodotPulse: .NET support not detected. Core sampling will be disabled.")

func _exit_tree() -> void:
	# Unregister the C# autoload
	remove_autoload_singleton("GodotPulse")

	# Remove input action (clear events first for complete cleanup)
	if InputMap.has_action(TOGGLE_ACTION):
		InputMap.action_erase_events(TOGGLE_ACTION)
		InputMap.erase_action(TOGGLE_ACTION)

func _register_input_action() -> void:
	if InputMap.has_action(TOGGLE_ACTION):
		# Check if action has expected events, warn if different
		var events = InputMap.action_get_events(TOGGLE_ACTION)
		var has_f3 = false
		for evt in events:
			if evt is InputEventKey and evt.keycode == KEY_F3:
				has_f3 = true
				break
		if not has_f3:
			push_warning("GodotPulse: Input action '%s' already exists with different key binding. " % TOGGLE_ACTION +
						 "Please ensure F3 is bound to toggle the overlay, or reassign in Project Settings.")
		return

	InputMap.add_action(TOGGLE_ACTION)

	var key_event := InputEventKey.new()
	key_event.keycode = KEY_F3
	InputMap.action_add_event(TOGGLE_ACTION, key_event)

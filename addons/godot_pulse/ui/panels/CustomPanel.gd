extends PanelContainer

@onready var _container: VBoxContainer = %MetricContainer
var _rows: Dictionary = {}
var _pulse = null

func _ready() -> void:
	_pulse = get_node_or_null("/root/GodotPulse")

func _process(_delta: float) -> void:
	if not is_visible_in_tree():
		return

	if not _pulse: return
	
	var names = _pulse.GetCustomMetricNames()
	var current_names = PackedStringArray(names)
	
	# Add new rows - O(n) using Dictionary lookup
	for name in current_names:
		if not _rows.has(name):
			var row = _create_metric_row(name)
			_container.add_child(row)
			_rows[name] = row
			
			# Use GetBufferAsPackedArray for safe GDScript interop (avoids CLR generic marshaling)
			var buffer = _pulse.GetBufferAsPackedArray("custom/" + name)
			if buffer != null and buffer.size() > 0:
				row.call("link_buffer", buffer)
			else:
				push_warning("GodotPulse: Custom metric buffer '%s' is empty or missing" % name)

	# Update rows
	for name in _rows:
		_rows[name].call("update_data")

	# Cleanup old rows - O(n) using Dictionary difference
	var names_set = {}
	for name in current_names:
		names_set[name] = true
	
	var to_remove = []
	for name in _rows:
		if not names_set.has(name):
			to_remove.append(name)
			
	for name in to_remove:
		_rows[name].queue_free()
		_rows.erase(name)

func _create_metric_row(name: String) -> HBoxContainer:
	var row = HBoxContainer.new()
	row.set_script(load("res://addons/godot_pulse/ui/panels/MetricRow.gd"))
	row.set_meta("metric_name", name)
	return row

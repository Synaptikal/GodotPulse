extends HBoxContainer

var _label: Label
var _value_label: Label
var _graph: Control
var _buffer = null
var _name: String

func _init() -> void:
	size_flags_horizontal = SIZE_EXPAND_FILL
	
	_label = Label.new()
	_label.size_flags_horizontal = SIZE_EXPAND_FILL
	_label.add_theme_font_size_override("font_size", 12)
	
	_value_label = Label.new()
	_value_label.custom_minimum_size = Vector2(40, 0)
	_value_label.add_theme_font_size_override("font_size", 12)
	
	_graph = Control.new()
	_graph.custom_minimum_size = Vector2(50, 20)
	# Dynamically construct path to MiniGraph in sibling widgets directory
	var panels_dir = get_script().get_path().get_basename()
	var widgets_dir = panels_dir.get_basename()  # Go up to ui directory
	var mini_graph_path = widgets_dir + "/widgets/MiniGraph.gd"
	_graph.set_script(load(mini_graph_path))
	
	add_child(_label)
	add_child(_value_label)
	add_child(_graph)

func _ready() -> void:
	if has_meta("metric_name"):
		_name = get_meta("metric_name")
		_label.text = _name

func link_buffer(buffer) -> void:
	if buffer == null:
		push_warning("MetricRow: Attempted to link null buffer")
		return
	_buffer = buffer
	_graph.link_buffer(buffer, 0, 100, Color.CYAN)

func update_data() -> void:
	if _buffer != null:
		# Handle both PackedFloat32Array and RingBuffer<T>
		if _buffer is PackedFloat32Array:
			if _buffer.size() > 0:
				_value_label.text = "%.1f" % _buffer[_buffer.size() - 1]
		elif _buffer.has_method("Get") and _buffer.has_method("Count"):
			# RingBuffer compatibility (if still using old API)
			if _buffer.Count > 0:
				_value_label.text = "%.1f" % _buffer.Get(_buffer.Count - 1)

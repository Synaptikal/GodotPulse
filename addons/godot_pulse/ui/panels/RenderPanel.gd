extends PanelContainer

@onready var _draw_calls_label: Label = %DrawCallsLabel
@onready var _vram_label: Label = %VramLabel
@onready var _draw_calls_graph: Control = %DrawCallsGraph

var _pulse = null

func _ready() -> void:
	_pulse = get_node_or_null("/root/GodotPulse")
	if _pulse:
		# Use DefaultMetricBufferSize constant (128)
		_pulse.RegisterBuffer("draw_calls", 128)
		# Use GetBufferAsPackedArray for safe GDScript interop (avoids CLR generic marshaling)
		var dc_buffer = _pulse.GetBufferAsPackedArray("draw_calls")
		if dc_buffer != null and dc_buffer.size() > 0:
			_draw_calls_graph.link_buffer(dc_buffer, 0, 2000, Color.ORANGE)
		else:
			push_warning("GodotPulse: Draw calls buffer is empty or missing")

func _process(_delta: float) -> void:
	if not is_visible_in_tree():
		return

	var dc = Performance.get_monitor(Performance.RENDER_TOTAL_DRAW_CALLS_IN_FRAME)
	var vram = Performance.get_monitor(Performance.RENDER_VIDEO_MEM_USED) / 1024.0 / 1024.0

	if _pulse:
		# Update graph with latest snapshot (safe GDScript interop via PackedFloat32Array)
		var dc_buffer = _pulse.GetBufferAsPackedArray("draw_calls")
		if dc_buffer != null and dc_buffer.size() > 0:
			_draw_calls_graph.link_buffer(dc_buffer, 0, 2000, Color.ORANGE)

	_draw_calls_label.text = "Draw Calls: %d" % dc
	_vram_label.text = "VRAM: %.1f MB" % vram

	if dc > 1500: _draw_calls_label.modulate = Color.RED
	elif dc > 500: _draw_calls_label.modulate = Color.YELLOW
	else: _draw_calls_label.modulate = Color.WHITE

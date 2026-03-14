@tool
extends Control

var _buffer = null
var _line_color: Color = Color(0.2, 0.8, 0.2)
var _min_val: float = 0.0
var _max_val: float = 100.0
var _auto_scale: bool = false
var _padding: float = 0.1  # 10% padding for auto-scale

func link_buffer(buffer, min_val: float = 0.0, max_val: float = 100.0, color: Color = Color.SPRING_GREEN, auto_scale: bool = false) -> void:
	if buffer == null:
		push_warning("MiniGraph: Attempted to link null buffer")
		return
	_buffer = buffer
	_min_val = min_val
	_max_val = max_val
	_line_color = color
	_auto_scale = auto_scale

func _process(_delta: float) -> void:
	queue_redraw()

func _draw() -> void:
	if _buffer == null:
		return

	var is_packed := _buffer is PackedFloat32Array
	var count := 0
	var max_size := 1

	if is_packed:
		count = _buffer.size()
		max_size = max(1, count)
	elif _buffer.has_method("Count") and _buffer.has_method("Get") and _buffer.has_method("MaxSize"):
		count = int(_buffer.Count)
		max_size = int(_buffer.MaxSize)
	elif typeof(_buffer) == TYPE_ARRAY:
		count = _buffer.size()
		max_size = max(1, count)
	else:
		push_warning("MiniGraph: Unsupported buffer type; expected PackedFloat32Array or RingBuffer-like object")
		return

	if count < 2:
		return

	var size = get_size()
	var points = PackedVector2Array()
	
	# Calculate min/max for auto-scaling
	var min_val = _min_val
	var max_val = _max_val
	if _auto_scale and count > 0:
		var first_val = _buffer[0] if is_packed else _buffer.Get(0)
		min_val = first_val
		max_val = first_val
		for i in range(1, count):
			var val = _buffer[i] if is_packed else _buffer.Get(i)
			min_val = min(min_val, val)
			max_val = max(max_val, val)
		# Add padding to prevent flat lines
		var range_val = max_val - min_val
		if range_val < 0.001:
			range_val = 0.001
		min_val -= range_val * _padding
		max_val += range_val * _padding
	
	var x_step = size.x / float(max_size - 1)
	
	for i in range(count):
		var val = _buffer[i] if is_packed else _buffer.Get(i)
		var t = remap(val, min_val, max_val, 0.0, 1.0)
		t = clamp(t, 0.0, 1.0)
		var y = size.y * (1.0 - t)
		points.append(Vector2(i * x_step, y))

	if points.size() > 1:
		draw_polyline(points, _line_color, 1.5, true)

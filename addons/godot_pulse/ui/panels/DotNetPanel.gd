extends PanelContainer

@onready var _heap_label: Label = %HeapLabel
@onready var _collections_label: Label = %CollectionsLabel
@onready var _pause_label: Label = %PauseLabel
@onready var _heap_graph: Control = %HeapGraph

var _pulse = null

func _ready() -> void:
	_pulse = get_node_or_null("/root/GodotPulse")
	if _pulse:
		# Use GetBufferAsPackedArray for safe GDScript interop (avoids CLR generic marshaling)
		var heap_buffer = _pulse.GetBufferAsPackedArray("dotnet_heap")
		if heap_buffer != null and heap_buffer.size() > 0:
			_heap_graph.link_buffer(heap_buffer, 0, 512, Color.DODGER_BLUE)
		else:
			push_warning("GodotPulse: Heap buffer is empty or missing")

func _process(_delta: float) -> void:
	if not is_visible_in_tree():
		return

	if not _pulse: return
	
	var heap_mb = _pulse.ManagedHeapBytes / 1024.0 / 1024.0
	_heap_label.text = "Heap: %.1f MB" % heap_mb
	_collections_label.text = "GC: %d/%d/%d" % [_pulse.Gen0Collections, _pulse.Gen1Collections, _pulse.Gen2Collections]
	
	var pause = _pulse.LastGcPauseMs
	if pause > 0:
		_pause_label.text = "Pause: %.2f ms" % pause
		_pause_label.modulate = Color.ORANGE_RED
	else:
		_pause_label.text = "Pause: 0 ms"
		_pause_label.modulate = Color.WHITE

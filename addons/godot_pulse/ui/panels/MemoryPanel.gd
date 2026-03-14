extends PanelContainer

@onready var _static_mem_label: Label = %StaticMemLabel
@onready var _objects_label: Label = %ObjectsLabel
@onready var _nodes_label: Label = %NodesLabel
@onready var _orphans_label: Label = %OrphansLabel

func _process(_delta: float) -> void:
	if not is_visible_in_tree():
		return

	var stat = Performance.get_monitor(Performance.MEMORY_STATIC) / 1024.0 / 1024.0
	var objs = Performance.get_monitor(Performance.OBJECT_COUNT)
	var nodes = Performance.get_monitor(Performance.OBJECT_NODE_COUNT)
	var orphans = Performance.get_monitor(Performance.OBJECT_ORPHAN_NODE_COUNT)

	_static_mem_label.text = "Static Mem: %.1f MB" % stat
	_objects_label.text = "Objects: %d" % objs
	_nodes_label.text = "Nodes: %d" % nodes
	_orphans_label.text = "Orphans: %d" % orphans

	if orphans > 0: _orphans_label.modulate = Color.YELLOW
	else: _orphans_label.modulate = Color.WHITE

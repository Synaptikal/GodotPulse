extends PanelContainer

@onready var _log_container: VBoxContainer = %LogContainer
var _last_event_count: int = 0
var _pulse = null

func _ready() -> void:
	_pulse = get_node_or_null("/root/GodotPulse")

func _process(_delta: float) -> void:
	if not is_visible_in_tree():
		return

	if not _pulse: return
	
	var events = _pulse.Events
	var current_count = events.Count
	
	# Handle ring buffer wrap-around (Count decreased due to buffer overflow)
	if current_count < _last_event_count:
		# Buffer wrapped - clear UI and show all current events
		for child in _log_container.get_children():
			child.queue_free()
		_last_event_count = 0
	
	# Add new events
	if current_count > _last_event_count:
		for i in range(_last_event_count, current_count):
			var evt = events.Get(i)
			_add_event_row(evt)
		_last_event_count = current_count

	# Limit UI events - uses GodotPulse.MaxUiEvents constant
	while _log_container.get_child_count() > _pulse.MaxUiEvents:
		var child = _log_container.get_child(0)
		_log_container.remove_child(child)
		child.queue_free()

func _add_event_row(evt) -> void:
	var label = Label.new()
	label.text = "[%.2f] %s" % [evt.Timestamp, evt.Message]
	label.clip_text = true
	label.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	label.add_theme_font_size_override("font_size", 10)

	# evt.Level is an enum from C#
	match int(evt.Level):
		1: label.modulate = Color.YELLOW # Warning
		2: label.modulate = Color.RED    # Critical
		_: label.modulate = Color.WHITE

	_log_container.add_child(label)

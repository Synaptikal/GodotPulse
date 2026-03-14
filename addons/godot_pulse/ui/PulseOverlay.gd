extends Control

@onready var _fps_label: Label = %FpsLabel
@onready var _ms_label: Label = %MsLabel
@onready var _fps_graph: Control = %FpsGraph
@onready var _main_panel: PanelContainer = $MainPanel
@onready var _spike_indicator: ColorRect = %SpikeIndicator

var _theme_path: String
var _pulse = null
var _spike_tween: Tween = null

func _ready() -> void:
	_pulse = get_node_or_null("/root/GodotPulse")
	if _pulse:
		# Use GetBufferAsPackedArray for safe GDScript interop (avoids CLR generic marshaling)
		var fps_buffer = _pulse.GetBufferAsPackedArray("fps")
		if fps_buffer != null and fps_buffer.size() > 0:
			_fps_graph.link_buffer(fps_buffer, 0, 120, Color.SPRING_GREEN)
		else:
			push_warning("GodotPulse: FPS buffer is empty or missing")
	
	_apply_theme()

func _apply_theme() -> void:
	if not _pulse: return

	_theme_path = _pulse.Config.ThemePath
	
	# Try to load custom theme with error handling
	_load_custom_theme()

	modulate.a = _pulse.Config.Opacity
	scale = Vector2(_pulse.Config.Scale, _pulse.Config.Scale)

func _load_custom_theme() -> void:
	if not FileAccess.file_exists(_theme_path):
		push_warning("GodotPulse: Theme file not found at %s. Using default theme." % _theme_path)
		_log_theme_error("Theme file not found: " + _theme_path)
		return

	var theme_res = ResourceLoader.load(_theme_path, "Theme", ResourceLoader.CACHE_MODE_REUSE)
	if theme_res is Theme:
		_main_panel.theme = theme_res
		if _pulse:
			_pulse.LogEvent("Theme loaded: " + _theme_path, _pulse.EventLevel.Info)
	else:
		push_warning("GodotPulse: Failed to load theme from %s. Using default theme." % _theme_path)
		_log_theme_error("Invalid theme resource: " + _theme_path)

func _log_theme_error(message: String) -> void:
	# Log to GodotPulse event system if available
	if _pulse and _pulse.has_method("LogEvent"):
		_pulse.LogEvent("Theme Error: " + message, _pulse.EventLevel.Warning)

func _process(_delta: float) -> void:
	if not is_visible_in_tree():
		return

	if not _pulse: return
	
	_fps_label.text = "FPS: %.1f" % _pulse.CurrentFps
	_ms_label.text = "%.2f ms" % _pulse.ProcessTime

	# Color coding FPS based on Config.TargetFps
	var target = _pulse.Config.TargetFps
	var warning_threshold = target * 0.9  # 90% of target (e.g., 54 for 60 FPS)
	var critical_threshold = target * 0.5  # 50% of target (e.g., 30 for 60 FPS)
	
	if _pulse.CurrentFps >= warning_threshold:
		_fps_label.modulate = Color.LIME_GREEN
	elif _pulse.CurrentFps >= critical_threshold:
		_fps_label.modulate = Color.YELLOW
	else:
		_fps_label.modulate = Color.RED

	# GC Spike Visualization - reuse tween to prevent accumulation
	if _pulse.LastGcPauseMs > 0:
		if _spike_tween == null or not _spike_tween.is_valid():
			_spike_indicator.modulate.a = 0.5
			_spike_tween = create_tween()
			_spike_tween.tween_property(_spike_indicator, "modulate:a", 0.0, 0.5)


func _exit_tree() -> void:
	# Clean up tween if valid
	if _spike_tween != null and _spike_tween.is_valid():
		_spike_tween.kill()
	_spike_tween = null

extends PanelContainer

@onready var _bodies_label: Label = %BodiesLabel
@onready var _pairs_label: Label = %PairsLabel
@onready var _islands_label: Label = %IslandsLabel
@onready var _title_label: Label = %TitleLabel

var _use_3d_physics: bool = true

func _ready() -> void:
	# Detect which physics engine is active
	_use_3d_physics = _detect_physics_engine()
	_update_title()

func _detect_physics_engine() -> bool:
	# Check actual physics server state for more accurate detection
	# Get active object counts to determine which physics engine is actually in use
	var bodies_3d = Performance.get_monitor(Performance.PHYSICS_3D_ACTIVE_OBJECTS)
	var bodies_2d = Performance.get_monitor(Performance.PHYSICS_2D_ACTIVE_OBJECTS)
	
	# If 2D has active bodies and 3D doesn't, prefer 2D
	if bodies_2d > 0 and bodies_3d == 0:
		return false
	
	# Check project settings as fallback
	var physics_3d_enabled = ProjectSettings.get_setting("physics/3d/run_on_separate_thread") != null
	var physics_2d_enabled = ProjectSettings.get_setting("physics/2d/run_on_separate_thread") != null
	
	# If only 2D physics settings exist and no 3D bodies, use 2D
	if not physics_3d_enabled and physics_2d_enabled and bodies_3d == 0:
		return false
	
	# Default to 3D (most common case, or if both are active)
	return true

func _update_title() -> void:
	if _title_label:
		_title_label.text = "PHYSICS (" + ("3D" if _use_3d_physics else "2D") + ")"

func _process(_delta: float) -> void:
	if not is_visible_in_tree():
		return

	var bodies: int
	var pairs: int
	var islands: int

	if _use_3d_physics:
		bodies = Performance.get_monitor(Performance.PHYSICS_3D_ACTIVE_OBJECTS)
		pairs = Performance.get_monitor(Performance.PHYSICS_3D_COLLISION_PAIRS)
		islands = Performance.get_monitor(Performance.PHYSICS_3D_ISLAND_COUNT)
	else:
		bodies = Performance.get_monitor(Performance.PHYSICS_2D_ACTIVE_OBJECTS)
		pairs = Performance.get_monitor(Performance.PHYSICS_2D_COLLISION_PAIRS)
		islands = Performance.get_monitor(Performance.PHYSICS_2D_ISLAND_COUNT)

	_bodies_label.text = "Active Bodies: %d" % bodies
	_pairs_label.text = "Collision Pairs: %d" % pairs
	_islands_label.text = "Islands: %d" % islands

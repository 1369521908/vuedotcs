@tool
extends EditorPlugin

func _enter_tree():
	print("[vuedotcs] Plugin loaded")

func _exit_tree():
	print("[vuedotcs] Plugin unloaded")
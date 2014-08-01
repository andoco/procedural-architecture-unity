using UnityEngine;
using System;
using System.Collections.Generic;

public class MenuItem
{
	public MenuItem(string name, Action action)
	{
		this.Name = name;
		this.SelectedAction = action;
	}

	public string Name { get; private set; }

	public Action SelectedAction { get; private set; }
}

public class Menu : MonoBehaviour {
	
	private List<MenuItem> items = new List<MenuItem>();
	private MenuItem closeItem;
	private Vector2 scrollPos;

	public bool autoDraw = true;
	public bool guiLayout = false;
	public bool autoClose = true;
	public bool isOpen;

	void Awake()
	{
		this.AddItem("Close", this.Close);
		this.closeItem = this.items[0];
	}

	void OnGUI()
	{
		if (this.isOpen)
		{
			this.DrawOpenMenu();
		}
	}

	public void AddItem(string name, Action action)
	{
		if (this.closeItem != null)
			this.items.RemoveAt(this.items.Count - 1);

		this.items.Add(new MenuItem(name, action));

		if (this.closeItem != null)
			this.items.Add(this.closeItem);
	}

	public void Close()
	{
		this.isOpen = false;
	}

	public void DrawMenuButton()
	{
		if (!this.isOpen)
			this.DrawClosedMenu();
	}

	private void DrawClosedMenu()
	{
		if (this.guiLayout)
		{
			if (GUILayout.Button("Menu"))
			{
				this.isOpen = true;
			}
		}
		else
		{
			var style = new GUIStyle(GUI.skin.button);
			style.fontSize = Screen.height / 20;
			var height = Screen.height/20;
			var width = Screen.width/10;
			if (GUI.Button(new Rect(20,Screen.height-(height + 20),width,height), "Menu", style))
			{
				this.isOpen = true;
			}
		}
	}

	private void DrawOpenMenu()
	{		
		var style = new GUIStyle(GUI.skin.button);
		style.fontSize = Screen.height / 20;
		
		GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
		GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.BeginVertical();

		GUILayout.FlexibleSpace();
		this.scrollPos = GUILayout.BeginScrollView(this.scrollPos, GUILayout.Width(Screen.width * 0.5f), GUILayout.Height(Screen.height * 0.75f));
		for (int i=0; i < this.items.Count; i++)
		{
			var item = this.items[i];
			
			if (GUILayout.Button(item.Name, style))
			{
				this.OnItemSelected(item);
			}
		}
		GUILayout.EndScrollView();
		GUILayout.FlexibleSpace();

		GUILayout.EndVertical();
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.EndArea();
	}

	private void OnItemSelected(MenuItem item)
	{
		item.SelectedAction();

		if (this.autoClose)
		{
			this.Close();
		}
	}
}

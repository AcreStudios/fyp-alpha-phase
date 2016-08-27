using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(WPN_WeaponSystem))]
public class Editor_WeaponHandler : Editor 
{
	// Script
	private WPN_WeaponSystem weapon;

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		weapon = (WPN_WeaponSystem)target;

		EditorGUILayout.LabelField("Weapon Tools (Use only during runtime!)");
		if(GUILayout.Button("Save current equip location"))
		{
			Transform weaponTrans = weapon.transform;
			Vector3 weaponPos = weaponTrans.localPosition;
			Vector3 weaponRot = weaponTrans.localEulerAngles;

			weapon.weaponSettings.equipPosition = weaponPos;
			weapon.weaponSettings.equipRotation = weaponRot;
		}
		if(GUILayout.Button("Save current unequip location"))
		{
			Transform weaponTrans = weapon.transform;
			Vector3 weaponPos = weaponTrans.localPosition;
			Vector3 weaponRot = weaponTrans.localEulerAngles;

			weapon.weaponSettings.unequipPosition = weaponPos;
			weapon.weaponSettings.unequipRotation = weaponRot;
		}

		EditorGUILayout.LabelField("Weapon Positioning");
		if(GUILayout.Button("Move to equip location"))
		{
			Transform weaponTrans = weapon.transform;
			weaponTrans.localPosition = weapon.weaponSettings.equipPosition;
			Quaternion rot = Quaternion.Euler(weapon.weaponSettings.equipRotation);
			weaponTrans.localRotation = rot;
		}
		if(GUILayout.Button("Move to unequip location"))
		{
			Transform weaponTrans = weapon.transform;
			weaponTrans.localPosition = weapon.weaponSettings.unequipPosition;
			Quaternion rot = Quaternion.Euler(weapon.weaponSettings.unequipRotation);
			weaponTrans.localRotation = rot;
		}
	}
}

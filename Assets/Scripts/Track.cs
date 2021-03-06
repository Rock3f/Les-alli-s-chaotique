﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public struct ScreenEnnemy {
	public string name;
	public GameObject[] ennemies;
}

public class Track : MonoBehaviour {

	public ScreenEnnemy[] allScreen;

	public Transform target;
	public float trackingDistance = 1f;
	public bool isCameraFix = true;
	public GameObject go;
	public GameObject ennemies;
	public GameObject croix;
	private float accumulateur = 0;
	public float frameDuration = 0.2f;
	private int nbClignotement = 0;
	public int maxClignotement = 4;
	private string actualScreenName;
	public int idCurrentScreen = 0;

	// Use this for initialization
	void Start () {	
	}
	
	// Update is called once per frame
	void Update () {
		
		actualScreenName = "screen" + idCurrentScreen;
		ScreenEnnemy screen = allScreen.FirstOrDefault(x => x.name == actualScreenName);
		
		if(screen.ennemies != null)
		{
			isCameraFix = !screen.ennemies.All(x => x == null);
		}
		else
		{
			isCameraFix = false;
		}
		

		//Réalisation du tracking de la caméra 
		//CAMERA
		if(!isCameraFix)
		{
			// Position vers laquelle la caméra doit tendre
			// attention à bien conserver la position Z actuelle
			// on ne poursuit que les positions x et y
			Vector3 targetPos = new Vector3 (
				target.position.x,
				this.transform.position.y,
				this.transform.position.z
			);

			// l'offset est un déplacement relatif 
			// On va appliquer à la position de la caméra
			// Il est initialisé à zéro (pas de mouvement)
			Vector3 moveOffset = Vector3.zero;

			// Si la cible s'éoigne de la distance de Tracking
			// Le offset va prendre une valeur pour déplacer la Caméra vers la cible
			// Attention, ici on utiliser Vector2.Distance plutôt que Vector3.Distance, 
			//		      car seule la distance des coordonnées x et y nous intéresse.
			//            Avec Vector3.Distance la distance en Z serait prise en compte aussi
			//            ce qui n'est pas souhaité.
			//            Les Vector3 passés en paramètre sont transformé automatique en Vector2.
			if (Vector2.Distance (target.position, this.transform.position) > trackingDistance) {
				// C'est la formule magique du easing joli et rapide à mettre en place
				// Chaque frame, on va déplacer la Caméra de 5% de la distance qui la sépare de la cible
				// La formule est offset = (cible - valeur) * pourcentageDeProgression
				// C'est équivalent à Mathf.Lerp(valeur, cible, pourcentageDeProgression);
				// Pour une Vector3, C'est Vector3.Lerp(position, destination, pourcentageDeProgression)
				moveOffset = (targetPos - this.transform.position) * 0.05f;
			}

			// Applique le déplacement à la position en additionnant les vecteurs
			// équivalent de <code>this.transform.position = this.transform.position + moveOffset;</code>
			this.transform.position += moveOffset;
			ennemies.transform.position += moveOffset;

			 accumulateur += Time.deltaTime;

			AudioSource source = gameObject.GetComponents<AudioSource>().FirstOrDefault(x => x.clip.name.Contains("bell"));
        	// vide l'accumulateur et fait avancer les frames        
        	while (accumulateur > frameDuration && frameDuration > 0) {
				if(!source.isPlaying && !go.activeSelf)
				{
					source.Play();
				}

				if(nbClignotement <= maxClignotement)
				{
					go.SetActive(!go.activeSelf);
				}
				else
				{
					go.SetActive(true);
				}
            
				nbClignotement++;
				accumulateur -= frameDuration;
        	}
		}
		else
		{
			go.SetActive(false);
		}		
	}
}

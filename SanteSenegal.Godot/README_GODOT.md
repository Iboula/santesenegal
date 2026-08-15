# 🎮 Projet Godot — SanteSenegal 3D

## Ouverture dans Godot

1. Ouvre **Godot Engine 4.x**
2. Clique sur **Import**
3. Sélectionne :
   ```
   C:\Users\iboul\Documents\PROJETS\SANTESENEGAL\SanteSenegal\SanteSenegal.Godot\project.godot
   ```
4. Clique sur **Edit**

## Structure du projet

```
SanteSenegal.Godot/
├── project.godot                 # Configuration projet
├── scenes/
│   ├── carte_3d.tscn             # 🗺️ Scène principale (terrain, caméra, lumières)
│   ├── marqueur_accident.tscn    # 🚨 Marqueur 3D rouge/orange
│   └── marqueur_hopital.tscn     # 🏥 Bâtiment 3D bleu avec croix
└── scripts/
    ├── carte_controller.gd       # Logique caméra + ajout dynamique des marqueurs
    └── data_bridge.gd            # Pont JavaScript ↔ Godot (web export)
```

## Ce qui est déjà construit

| Élément | Description |
|---------|-------------|
| **Carte3D** | Node3D principal avec caméra orbitale, lumière directionnelle avec ombres |
| **MarqueurAccident** | Cylindre coloré selon gravité (🔴 critique / 🟠 élevé / 🟡 moyen / 🟢 faible) + anneau lumineux au sol |
| **MarqueurHopital** | Cube bleu avec hauteur selon capacité + croix blanche sur le toit |
| **DataBridge** | Reçoit des données JSON depuis JavaScript (navigateur) et les injecte dans la scène |

## 🚀 Export Web (HTML5)

1. Dans Godot : **Project → Export**
2. Clique sur **Add → Web**
3. Configure :
   - **Export Path** : `C:\Users\iboul\Documents\PROJETS\SANTESENEGAL\SanteSenegal\SanteSenegal.Web\wwwroot\godot\`
   - **Threads** : Enabled (si supporté par le serveur)
   - **Canvas Resize Policy** : Adaptive
4. Clique sur **Export Project**

## 🔗 Intégration Blazor

Après export, ton fichier `index.html` Godot sera dans :
```
SanteSenegal.Web/wwwroot/godot/index.html
```

Tu peux l'intégrer dans Blazor via une `<iframe>` :

```razor
<iframe src="godot/index.html" style="width:100%;height:600px;border:none;" />
```

## 📡 Envoi de données depuis Blazor

Dans ton navigateur (via la console JS ou Blazor) :

```javascript
window.sendDataToGodot(JSON.stringify({
    accidents: [
        { lat: 14.7167, lng: -17.4677, gravite: "critique", victimes: 5 },
        { lat: 14.5000, lng: -17.0000, gravite: "moyen", victimes: 2 }
    ],
    hopitaux: [
        { lat: 14.7167, lng: -17.4677, nom: "Hôpital Principal", lits: 150 }
    ]
}));
```

Les marqueurs apparaîtront instantanément dans la scène 3D !

---

**Made with ❤️ for Senegal 🇸🇳**

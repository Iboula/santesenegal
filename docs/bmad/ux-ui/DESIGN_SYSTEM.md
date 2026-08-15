# SanteSenegal — Design System

**Agent** : UX/UI  
**Date** : 2026-08-13  
**Version** : 1.0

---

## 1. Philosophie visuelle

**"Santé accessible, confiance visuelle"**

- **Clarté** : L'information médicale doit être immédiatement compréhensible
- **Confiance** : Des couleurs rassurantes, pas d'agressivité
- **Accessibilité** : Contrastes forts, tailles de polices généreuses
- **Proximité** : Des tons chaleureux, pas institutionnels froids

---

## 2. Couleurs

### Palette primaire — Santé & Confiance

```
┌─────────────────────────────────────────────────────────────────┐
│  PRIMARY TEAL (Santé, soin, fraîcheur)                          │
│                                                                 │
│  ████████  teal-50   #E6F7F5   Fond clair                      │
│  ████████  teal-100  #B3EBE4   Survol léger                    │
│  ████████  teal-200  #80DFD3   Bordures                        │
│  ████████  teal-300  #4DD3C2   État focus                      │
│  ████████  teal-400  #26C9B5   Accent secondaire               │
│  ████████  teal-500  #00BFAD   ███ PRIMARY ███                 │
│  ████████  teal-600  #00A894   Hover primaire                  │
│  ████████  teal-700  #00927B   Actif                           │
│  ████████  teal-800  #007B62   Texte sur fond clair            │
│  ████████  teal-900  #00644A   Texte foncé                     │
└─────────────────────────────────────────────────────────────────┘
```

### Palette secondaire — Actions & Alertes

```
┌─────────────────────────────────────────────────────────────────┐
│  SUCCESS GREEN (Validation, confirmation)                       │
│  ████████  success-500  #22C55E   Succès, disponible           │
│  ████████  success-100  #DCFCE7   Fond succès                  │
│                                                                 │
│  WARNING ORANGE (Attention, rappel)                             │
│  ████████  warning-500  #F59E0B   Alertes, créneaux limités    │
│  ████████  warning-100  #FEF3C7   Fond warning                 │
│                                                                 │
│  ERROR RED (Annulation, indisponible)                           │
│  ████████  error-500    #EF4444   Erreur, créneau pris         │
│  ████████  error-100    #FEE2E2   Fond erreur                  │
│                                                                 │
│  INFO BLUE (Information, lien)                                  │
│  ████████  info-500     #3B82F6   Liens, info                  │
│  ████████  info-100     #DBEAFE   Fond info                    │
└─────────────────────────────────────────────────────────────────┘
```

### Palette neutre — Texte & Fond

```
┌─────────────────────────────────────────────────────────────────┐
│  ████████  white        #FFFFFF   Fond principal               │
│  ████████  gray-50      #F9FAFB   Fond alterné                 │
│  ████████  gray-100     #F3F4F6   Fond section                 │
│  ████████  gray-200     #E5E7EB   Bordures, séparateurs       │
│  ████████  gray-300     #D1D5DB   Icônes inactives            │
│  ████████  gray-400     #9CA3AF   Placeholder text             │
│  ████████  gray-500     #6B7280   Texte secondaire             │
│  ████████  gray-600     #4B5563   Texte corps                 │
│  ████████  gray-700     #374151   Texte important              │
│  ████████  gray-800     #1F2937   Titres                      │
│  ████████  gray-900     #111827   Texte principal              │
│  ████████  black        #000000   Texte accentué               │
└─────────────────────────────────────────────────────────────────┘
```

### Couleurs spécifiques Mobile Money

```
┌─────────────────────────────────────────────────────────────────┐
│  WAVE        #1DA1F2   (Bleu vif)                              │
│  ORANGE MONEY #FF6600   (Orange)                               │
│  FREE MONEY   #E3000F   (Rouge)                                │
└─────────────────────────────────────────────────────────────────┘
```

---

## 3. Typographie

### Police principale : Inter (Google Fonts)

**Pourquoi Inter ?**
- Excellente lisibilité à petites tailles
- Chiffres tabulaires (prix, horaires)
- Support multilingue complet (français + caractères spéciaux)
- 9 weights disponibles

### Échelle typographique

| Token | Taille | Line-height | Weight | Usage |
|-------|--------|-------------|--------|-------|
| **Display** | 32px | 40px | Bold (700) | Titre écran |
| **H1** | 28px | 36px | Bold (700) | Section principale |
| **H2** | 22px | 30px | SemiBold (600) | Sous-section |
| **H3** | 18px | 26px | SemiBold (600) | Card title |
| **Body** | 16px | 24px | Regular (400) | Texte principal |
| **Body-sm** | 14px | 20px | Regular (400) | Description |
| **Caption** | 12px | 16px | Medium (500) | Labels, badges |
| **Overline** | 10px | 14px | SemiBold (600) | Tags, catégories |

### Règles d'accessibilité
- **Ratio contraste minimum** : 4.5:1 pour le texte normal (WCAG AA)
- **Ratio contraste idéal** : 7:1 pour le texte important (WCAG AAA)
- **Pas de texte < 12px** jamais
- **Line-height minimum** : 1.5x pour les paragraphes

---

## 4. Espacements (Spacing Scale)

```
Base unit = 4px

4px   (1x)   ──  Micro-espace (icon + label)
8px   (2x)   ──  Petit (padding interne button)
12px  (3x)   ──  Compact (gap entre chips)
16px  (4x)   ──  Standard (marge latérale)
20px  (5x)   ──  Moyen (gap cards)
24px  (6x)   ──  Large (espace entre sections)
32px  (8x)   ──  XL (header padding)
40px  (10x)  ──  XXL (hero spacing)
48px  (12x)  ──  Section break
```

---

## 5. Composants

### 5.1 Boutons

#### Primary Button
```
┌────────────────────────────┐
│                            │
│   [       Label       ]    │
│                            │
│   Fond : teal-500          │
│   Texte : white            │
│   Height : 48px            │
│   Radius : 12px            │
│   Padding : 0 24px         │
│   Font : 16px SemiBold     │
│                            │
│   Hover : teal-600         │
│   Active : teal-700        │
│   Disabled : gray-300      │
│                            │
└────────────────────────────┘
```

#### Secondary Button
```
┌────────────────────────────┐
│                            │
│   [       Label       ]    │
│                            │
│   Fond : white             │
│   Bordure : 1px teal-500   │
│   Texte : teal-500         │
│   Height : 44px            │
│   Radius : 8px             │
│                            │
└────────────────────────────┘
```

#### Mobile Money Button
```
┌────────────────────────────┐
│  [🌊]  Payer avec Wave     │
│                            │
│  Fond : #1DA1F2            │
│  Texte : white             │
│  Icon : left-aligned       │
│  Height : 56px             │
│  Radius : 12px             │
│                            │
└────────────────────────────┘
```

### 5.2 Input Fields

```
┌─────────────────────────────────┐
│  Label                          │  ← 14px Medium, gray-700
│  ┌───────────────────────────┐  │
│  │ 🔍 Placeholder...       │  │  ← 16px Regular, gray-400
│  └───────────────────────────┘  │
│  Helper text (optional)         │  ← 12px Regular, gray-500
└─────────────────────────────────┘

États :
- Default : border gray-200
- Focus : border teal-500, shadow teal-100
- Error : border error-500, text error-500
- Disabled : bg gray-50, text gray-400
```

### 5.3 Cards

```
┌─────────────────────────────────┐
│                                 │
│  ┌───────────────────────────┐  │
│  │                           │  │  ← Image (ratio 16:9)
│  │      [Image]              │  │
│  │                           │  │
│  ├───────────────────────────┤  │
│  │  🏥 Nom Structure         │  │  ← 18px SemiBold
│  │  ⭐ 4.8 · 📍 2 km         │  │  ← 14px Regular
│  │  Pédiatrie · Généraliste  │  │  ← 12px Regular
│  │                           │  │
│  └───────────────────────────┘  │
│                                 │
│  Shadow : 0 2px 8px rgba(0,0,0,0.08)
│  Radius : 16px
│  Background : white
│  Padding : 0 (image) + 16px (content)
│                                 │
└─────────────────────────────────┘
```

### 5.4 Badges / Tags

| Type | Fond | Texte | Usage |
|------|------|-------|-------|
| Disponible | success-100 | success-700 | Créneau libre |
| Complet | error-100 | error-700 | Créneau plein |
| En attente | warning-100 | warning-700 | Paiement en cours |
| Confirmé | teal-100 | teal-800 | RDV confirmé |
| Spécialité | gray-100 | gray-700 | Tag médecin |

```
┌──────────┐
│ Pédiatrie │  ← 12px Medium, radius=999px (pill)
└──────────┘
```

### 5.5 Bottom Navigation

```
┌─────────────────────────────────┐
│  ⭐      📅      👤      ⚙️      │
│  Fav     RDV    Profil  Plus    │
│                                 │
│  Height : 64px + safe area      │
│  Background : white             │
│  Shadow : top 1px gray-200      │
│  Active : teal-500 + label      │
│  Inactive : gray-400            │
└─────────────────────────────────┘
```

---

## 6. Icônes

### Bibliothèque : Phosphor Icons

**Pourquoi Phosphor ?**
- Style consistent (stroke-based)
- 6 weights (Thin à Bold)
- Support React/Vue/Flutter natif
- Gratuit et open-source

### Mapping icônes → Actions

| Action | Icône | Weight |
|--------|-------|--------|
| Accueil | House | Regular |
| Rechercher | MagnifyingGlass | Regular |
| Rendez-vous | CalendarCheck | Regular |
| Profil | User | Regular |
| Paramètres | Gear | Regular |
| Localisation | MapPin | Fill |
| Téléphone | Phone | Fill |
| Email | Envelope | Regular |
| Étoile | Star | Fill (jaune) |
| Succès | CheckCircle | Fill (vert) |
| Erreur | XCircle | Fill (rouge) |
| Info | Info | Regular |
| Retour | ArrowLeft | Regular |
| Favoris | Heart | Fill (rouge) |
| Partager | ShareNetwork | Regular |
| Notification | Bell | Regular |
| Paiement | CreditCard | Regular |
| Wave | Wave (custom) | Fill |
| Orange Money | Orange (custom) | Fill |
| Free Money | Free (custom) | Fill |

---

## 7. Animations & Micro-interactions

### Durées
| Type | Durée | Easing |
|------|-------|--------|
| Micro (hover, focus) | 150ms | ease-out |
| Standard (transitions) | 200ms | ease-in-out |
| Émergence (modals) | 300ms | cubic-bezier(0.4, 0, 0.2, 1) |
| Succès (confirmation) | 500ms | bounce |

### Patterns
- **Button press** : scale(0.97) on active
- **Card hover** : translateY(-2px) + shadow increase
- **Page transition** : slide from right (push), fade (modal)
- **Pull-to-refresh** : rotate spinner 360°
- **Skeleton loading** : shimmer animation (gray-200 → gray-100 → gray-200)

---

## 8. Responsive Breakpoints

| Nom | Largeur | Usage |
|-----|---------|-------|
| **Mobile** | 0 - 639px | Téléphones (principal) |
| **Tablet** | 640px - 1023px | Tablettes, petits laptops |
| **Desktop** | 1024px+ | Dashboard admin, laptop |

### Adaptations
- **Mobile** : Stack vertical, bottom nav, full-width cards
- **Tablet** : 2-col grid, side nav possible
- **Desktop** : 3-col grid, sidebar fixe, hover states

---

## 9. Accessibilité (a11y)

### Obligatoire
- [ ] **Touch targets** ≥ 48x48 dp
- [ ] **Contrast ratios** ≥ 4.5:1 (texte), ≥ 3:1 (UI components)
- [ ] **Focus indicators** visibles (outline teal-500)
- [ ] **Screen reader labels** sur tous les éléments interactifs
- [ ] **Reduced motion** support (`prefers-reduced-motion`)

### Recommandé
- [ ] **VoiceOver/TalkBack** testing
- [ ] **Font scaling** support (jusqu'à 200%)
- [ ] **Dark mode** (future v1.2)
- [ ] **RTL** support (arabe si expansion)

---

## 10. Ressources

### Fonts
- **Inter** : https://fonts.google.com/specimen/Inter

### Icônes
- **Phosphor Icons** : https://phosphoricons.com

### Outils de vérification
- **Contrast checker** : https://webaim.org/resources/contrastchecker/
- **A11y testing** : Chrome DevTools Lighthouse

---

**Ce Design System est vivant** : Il évoluera avec les retours utilisateurs et les tests.

**Prochaine étape** : Prototypage interactif (Figma/Blazor) ou développement frontend direct.

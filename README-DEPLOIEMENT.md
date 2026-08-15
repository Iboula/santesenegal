# 🚀 Guide de Déploiement — SanteSenegal

## Prérequis

- Docker + Docker Compose
- (Optionnel) VPS cloud (DigitalOcean, AWS Lightsail, Azure VM, OVH)
- (Optionnel) GitHub Actions pour CI/CD

---

## 🐳 Déploiement local avec Docker

```bash
# 1. Cloner le projet
git clone <repo-url>
cd SanteSenegal

# 2. Créer le fichier .env
JWT_SECRET="votre-cle-super-secrete-de-32-caracteres-min!"
JWT_ISSUER="SanteSenegal"
JWT_AUDIENCE="SanteSenegalMobile"

# 3. Lancer
sudo docker-compose up -d --build

# 4. Vérifier
sudo docker-compose ps
curl http://localhost:8080/swagger/v1/swagger.json
```

---

## ☁️ Déploiement sur VPS (Ubuntu)

### 1. Préparer le serveur

```bash
# Installer Docker
sudo apt update && sudo apt install -y docker.io docker-compose
sudo usermod -aG docker $USER

# Créer le dossier d'application
sudo mkdir -p /opt/santesenegal
sudo chown $USER:$USER /opt/santesenegal
```

### 2. Copier les fichiers

```bash
cd /opt/santesenegal

# Copier depuis votre machine locale
scp docker-compose.yml Dockerfile .env user@vps:/opt/santesenegal/
```

### 3. Lancer

```bash
cd /opt/santesenegal
docker-compose up -d
```

### 4. Configurer Nginx (reverse proxy + HTTPS)

```nginx
server {
    listen 80;
    server_name api.santesenegal.sn;

    location / {
        proxy_pass http://localhost:8080;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }
}
```

### 5. HTTPS avec Certbot

```bash
sudo apt install -y certbot python3-certbot-nginx
sudo certbot --nginx -d api.santesenegal.sn
```

---

## 🔒 Sécurité production

| Variable | Description |
|----------|-------------|
| `JWT_SECRET` | Clé secrète JWT (**minimum 32 caractères**) |
| `Jwt__Issuer` | Émetteur du token |
| `Jwt__Audience` | Audience du token |

⚠️ **Ne jamais commiter le `.env` !** Il est déjà dans `.gitignore`.

---

## 📱 Accès après déploiement

| Service | URL locale | URL production |
|---------|-----------|----------------|
| API Swagger | `http://localhost:8080/swagger` | `https://api.santesenegal.sn/swagger` |
| SignalR Hub | `ws://localhost:8080/hubs/alertes` | `wss://api.santesenegal.sn/hubs/alertes` |
| Blazor WASM | `https://localhost:7001` | `https://app.santesenegal.sn` |

---

## 🔄 Mise à jour

```bash
cd /opt/santesenegal
docker-compose pull
docker-compose up -d --remove-orphans
docker system prune -f
```

---

## 🆘 Dépannage

```bash
# Voir les logs
docker-compose logs -f api

# Redémarrer
docker-compose restart api

# Entrer dans le conteneur
docker exec -it santesenegal-api bash

# Vérifier la base SQLite
docker exec -it santesenegal-api ls -la /app/data/
```

---

**Déployé avec ❤️ pour le Sénégal 🇸🇳**

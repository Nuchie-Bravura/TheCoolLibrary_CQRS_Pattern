# SonarQube Theory - Deployment Options

## Option 1: SonarCloud (Open Source - Free)

```
GitHub Actions (GitHub server) open source cloud
    ↓
1. Clones your repo
2. Compiles the code
3. Runs tests
4. Analyzes with SonarScanner
    ↓
5. Sends ONLY the results → SonarCloud
    ↓
SonarCloud (cloud server)
    ↓
6. Processes results
7. Applies Quality Gate
8. Responds ✅ or ❌
    ↓
GitHub Actions
    ↓
9. Marks the check as successful/failed
```

---

## Option 2: SonarQube with VPS (Self-hosted)

```
GitHub Actions (GitHub server)
    ↓
1. Executes the .yml
2. The .yml says: "Connect to MY VPS at http://my-vps-ip:9000"
    ↓
    ↓ (via HTTP/HTTPS)
    ↓
VPS (your private server with public IP)
    ↓
3. GitHub Actions sends instruction to VPS: "Clone this repo and analyze it"
    ↓
4. SonarQube Server (on the VPS) clones your repo
5. SonarQube compiles the code
6. SonarQube runs tests
7. SonarQube performs complete analysis
8. SonarQube saves results in its DB (also on the VPS)
9. SonarQube responds to GitHub: ✅ or ❌
    ↓
GitHub Actions
    ↓
10. Marks the check as successful/failed
```

---

## Key Differences

| Feature | SonarCloud | SonarQube (VPS) |
|---------|-----------|-----------------|
| **Cost** | Free (public repos) | VPS hosting cost (~$10-50/month) |
| **Setup** | Zero configuration | Manual installation & maintenance |
| **Data Privacy** | Results stored in SonarCloud | Complete control on your server |
| **Scalability** | Unlimited | Limited by VPS resources |
| **Maintenance** | Managed by SonarSource | You manage updates/backups |
| **Best for** | Public projects, startups | Enterprise, compliance requirements |

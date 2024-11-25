# Progetto PhotoSi

## Descrizione

PhotoSiTest è un'applicazione di test sviluppata in .NET che gestisce ordini, prodotti, categorie di prodotto, utenti e indirizzi di consegna. 
Il sistema permette di creare e gestire ordini, ciascuno composto da uno o più prodotti, dove ogni prodotto appartiene a una categoria. 
Ogni ordine è associato a un utente e a un indirizzo di consegna, consentendo di tracciare e gestire le informazioni in modo completo. 
Il progetto è configurato per essere eseguito in un ambiente containerizzato utilizzando Podman e Podman Compose, 
ma può essere eseguito anche senza container per una configurazione locale.

## Requisiti

- **Podman**: Utilizzato per la gestione dei container.
- **Podman-compose**: Compatibile con Docker Compose, necessario per gestire i container in Podman.
- **.NET SDK 8.0**: Per compilare ed eseguire il progetto in locale.
- **PostgreSQL**: Database utilizzato dal progetto.

## Installazione e Avvio

### 1. Clona il Progetto

Poiché il progetto è sotto versionamento, puoi clonare il repository direttamente utilizzando Git. Esegui il comando:

```bash
git clone https://github.com/davidedg87/order_management.git
cd order_management
```

### 2. Configurazione dell'Ambiente

#### Con Docker/Podman:

1. **Configura il file `docker-compose.yml`**:
   - Assicurati che il file `docker-compose.yml` sia configurato correttamente per l'ambiente di sviluppo, come indicato nel seguente esempio:

    ```yaml
    version: '3.8'

    services:
      postgres-photosi:
        image: postgres:latest
        container_name: my-postgres-photosi
        environment:
          POSTGRES_USER: myuser
          POSTGRES_PASSWORD: mypassword
          POSTGRES_DB: photoSiTest-db
        ports:
          - "6543:5432"
        volumes:
          - postgres_data:/var/lib/postgresql/data
        networks:
          - app_network

      photositest_api:
        build:
          context: .  
          dockerfile: PhotoSiTest.API/Dockerfile  
        environment:
          ASPNETCORE_ENVIRONMENT: Development
          ConnectionStrings__DefaultConnection: "Host=postgres-photosi;Database=photoSiTest-db;Username=myuser;Password=mypassword;"
        ports:
          - "5002:8080"
        depends_on:
          - postgres-photosi
        networks:
          - app_network

    networks:
      app_network:
        driver: bridge

    volumes:
      postgres_data:
    ```

2. **Avvia i container con Podman**:
   - Assicurati di avere **Podman** e **Podman Compose** installati. Se non li hai, installali seguendo la documentazione ufficiale:
     - [Installazione Podman](https://podman.io/getting-started/installation)
     - [Installazione Podman Compose](https://github.com/containers/podman-compose)
   - Avvia i container eseguendo il seguente comando nella directory contenente il file `docker-compose.yml`:

    ```bash
    podman-compose -f docker-compose.yml up --build
    ```

   Questo comando costruirà e avvierà i container per il database PostgreSQL e l'API PhotoSi.

#### Senza Docker/Podman (In Locale):

Se preferisci eseguire il progetto senza Docker o Podman, esegui i seguenti passaggi:

1. **Configura PostgreSQL in locale**:
   - Installa PostgreSQL sul tuo sistema (seguendo le istruzioni per il tuo sistema operativo).
   - Crea un database chiamato `photoSiTest-db` e un utente con i permessi necessari.

2. **Configura la Stringa di Connessione**:
   - Modifica il file `appsettings.Development.json` per includere la stringa di connessione al tuo database locale PostgreSQL:

    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Host=localhost;Database=photoSiTest-db;Username=myuser;Password=mypassword;"
      }
    }
    ```

3. **Avvia il Progetto**:
   - Apri il progetto in Visual Studio o usa il comando `dotnet` per avviarlo:

    ```bash
    dotnet run
    ```

   Il progetto verrà avviato sulla porta predefinita (`http://localhost:5000`).

### 3. API Disponibili

Le seguenti API sono disponibili nell'applicazione:

- **Health**:
  - `GET /health`: Stato DB e DBContext associati 

- **Gestione Utenti**:
  - `GET /api/users/{id}`: Ottieni un utente specifico per ID.
  - `GET /api/users`: Ottieni tutti gli utenti.
  - `POST /api/users`: Crea un nuovo utente.
  - `PUT /api/users/{id}`: Modifica un utente esistente.
  - `DELETE /api/users/{id}`: Elimina un utente.
  - `POST /api/users/paginate`: Ricerca paginata utenti

- **Gestione Ordini**:
  - `GET /api/orders/{id}`: Ottieni un ordine specifico per ID, insieme ai dettagli dell'indirizzo, codice prodotto e nome utente.
  - `GET /api/orders`: Ottieni tutti gli ordini con i dettagli completi (indirizzo, codici dei prodotti e nome utente).
  - `POST /api/orders`: Crea un nuovo ordine. Verifica che l'indirizzo, l'utente e i prodotti esistano prima di creare l'ordine.
  - `PUT /api/orders/{id}`: Modifica un ordine esistente. Verifica che l'ordine, l'indirizzo, l'utente e i prodotti esistano prima dell'aggiornamento.
  - `DELETE /api/orders/{id}`: Elimina un ordine specifico.
  - `POST /api/orders/paginate`: Ricerca paginata ordini

- **Gestione Indirizzi**:
  - `GET /api/addresses/{id}`: Ottieni un indirizzo specifico per ID.
  - `GET /api/addresses`: Ottieni tutti gli indirizzi.
  - `POST /api/addresses`: Crea un nuovo indirizzo.
  - `PUT /api/addresses/{id}`: Modifica un indirizzo esistente. Verifica che l'indirizzo esista prima dell'aggiornamento.
  - `DELETE /api/addresses/{id}`: Elimina un indirizzo specifico, a condizione che non sia associato a ordini in stato "Pending" o "Processing".
  - `POST /api/addresses/paginate`: Ricerca paginata indirizzi

 - **Gestione Prodotti**:
  - `GET /api/products/{id}`: Ottieni un prodotto specifico per ID.
  - `GET /api/products`: Ottieni tutti i prodotti.
  - `POST /api/products`: Crea un nuovo prodotto. La categoria deve esistere e il prezzo non può essere inferiore a 0.
  - `PUT /api/products/{id}`: Modifica un prodotto esistente. Verifica che il prodotto esista, che la categoria sia valida e che il prezzo sia non negativo.
  - `DELETE /api/products/{id}`: Elimina un prodotto specifico, a condizione che non sia associato a ordini in stato "Pending" o "Processing".
  - `POST /api/products/paginate`: Ricerca paginata prodotti

- **Gestione Categorie Prodotti**:
  - `GET /api/productcategories/{id}`: Ottieni una categoria di prodotto specifica per ID.
  - `GET /api/productcategories`: Ottieni tutte le categorie di prodotto.
  - `POST /api/productcategories`: Crea una nuova categoria di prodotto.
  - `PUT /api/productcategories/{id}`: Modifica una categoria di prodotto esistente. Verifica che la categoria esista.
  - `DELETE /api/productcategories/{id}`: Elimina una categoria di prodotto. La categoria non può essere eliminata se ha prodotti associati.
  - `POST /api/productcategories/paginate`: Ricerca paginata categorie di prodotto

Porta di ascolto per le API:
- In ambiente locale (senza Docker):  
  HTTP: `http://localhost:5172`  
  HTTPS: `https://localhost:7101`

- In ambiente Docker:  
  La porta interna del container Docker è configurata su `5002`.

**Catalogo API Json** 
	Nel repo è disponibile il file "postman_collection.json" da importare in Postman per avere l'elenco delle API da invocare sul progetto [ impostato per funzionare sulla porta 5002 quindi con container docker running]

### 4. Testing

Per eseguire i test unitari:

1. **Esegui i Test con .NET CLI**:

    ```bash
    dotnet test
    ```

   Questo comando eseguirà tutti i test definiti nel progetto.

### 5. Configurazione di Rete e Volumi

I container PostgreSQL sono configurati per utilizzare un volume persistente per i dati nel file `docker-compose.yml`:

```yaml
volumes:
  postgres_data:

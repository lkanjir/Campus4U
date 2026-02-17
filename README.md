# Campus4U

## Opis domene
Aplikacija Campus4U usmjerena je na organizaciju života studenata koji borave u studentskom domu. Aplikacija ima cilj optimizirati svakodnevne aktivnosti studenata kao što su lakši pristup svim mogućnostima i prostorijama koje studentski kampus nudi, povećanje učinkovitosti korištenja zajedničkih prostora te unaprjeđenje suradnje među studentima. Aplikacija će studentima također ponuditi opciju pregleda dnevnog menija u studentskom restoranu. Aplikacija predstavlja desktop varijantu za mobilnu aplikaciju istog naziva koja se razvija za predmet RAMPU.

## Moje obveze
Oznaka | Naziv | Kratki opis 
------ | ----- | ----------- 
F01 | Registracija i prijava korisnika | Sustav omogućuje registraciju i prijavu putem korisničkog imena i lozinke. Na temelju uloge korisnika, sustav otvara različite prikaze i funkcionalnosti nakon uspješne prijave. U slučaju da račun korisnika ne postoji sustav korisniku nudi registraciju.
F05 | Slanje obavijesti putem e-maila | Sustav automatski šalje obavijesti korisnicima o nadolazećim rezervacijama, promjenama statusa prostora ili prijavama kvarova te drugim važnim događajima. Sustav generira poruke koje uključuju relevantne informacije, kao što su naziv prostora, datum i vrijeme rezervacije, status te promjene termina.
F09 | Studentski oglasnik | Korisnici mogu pregledavati i objavljivati obavijesti u studentskom oglasniku. Oglasnik služi za dijeljenje informacija o događanjima i aktivnostima u studentskom domu (npr. druženja, turniri, zabave). Svaka objava može sadržavati naslov, opis, datum i vrijeme događaja te opcionalnu sliku. Ostali korisnici mogu komentirati objave i označiti interes za dolazak. | Luka Kanjir

Moje ostale obaveze:
1. Postavljanje servera
2. Implementacija pohrane slika na serveru
3. Zaštita endpointova servera

## Sve funkcionalnosti

Oznaka | Naziv | Kratki opis
------ | ----- | ----------- 
F01 | Registracija i prijava korisnika | Sustav omogućuje registraciju i prijavu putem korisničkog imena i lozinke. Na temelju uloge korisnika, sustav otvara različite prikaze i funkcionalnosti nakon uspješne prijave. U slučaju da račun korisnika ne postoji sustav korisniku nudi registraciju.
F02 | Prikaz objekata i prostora | Korisnik ima mogućnost pregledavanja dostupnih učionica i teretane u studentskom domu. Prostore može pretraživati prema nazivu ili vrsti (učionica/teretana). Sustav prikazuje osnovne informacije o svakom prostoru, uključujući kapacitet i opremljenost.
F03 | Rezervacija prostorije | Korisnik odabire željenu učionicu ili teretanu te termin (datum i vrijeme) za koji želi kreirati rezervaciju. Korisnik ne može rezervirati prostoriju ako već postoji rezervacija za odabrano vrijeme. Ako je termin slobodan, korisnik može potvrditi rezervaciju, nakon čega ga aplikacija obavještava da je termin uspješno rezerviran. U tom trenutku, odabrani termin postaje nedostupan drugim studentima, a sustav automatski ažurira zauzetost prostora u stvarnom vremenu. 
F04 | Upravljanje rezervacijama | Korisnik može pristupiti popisu svojih aktivnih i prethodnih rezervacija. Odabirom neke buduće rezervacije, može izmijeniti datum ili vrijeme ako je novi termin slobodan ili u potpunosti otkazati rezervaciju prije početka termina. Nakon što korisnik izvrši izmjenu ili otkazivanje, sustav ga obavještava o uspješnoj promjeni te ažurira status prostora u stvarnom vremenu.
F05 | Slanje obavijesti putem e-maila | Sustav automatski šalje obavijesti korisnicima o nadolazećim rezervacijama, promjenama statusa prostora ili prijavama kvarova te drugim važnim događajima. Sustav generira poruke koje uključuju relevantne informacije, kao što su naziv prostora, datum i vrijeme rezervacije, status te promjene termina.
F06 | Upravljanje profilom korisnika | Korisnik može pregledavati i mijenjati podatke vlastitog profila te brisati neželjene neobavezne podatke. Podaci koji će biti prikazani su: korisničko ime, ime, prezime, broj sobe, korisnička slika, e-mail i broj telefona. Uz to korisnik može promijeniti lozinku profila. Obavezni podaci su podaci koji se unose kod registracije profila, a to su: korisničko ime i lozinka.
F07 | Upravljanje favoritima | Korisnik može izabrati prostorije, događaje i aktivnosti kao favorite. Time će korisnik primati vezane obavijesti. Uz to, korisniku će se u posebnoj sekciji prikazati svi favoriti kako bi im čim prije mogao pristupiti. 
F08 | Ocjenjivanje i davanje povratnih informacija | Korisnici mogu davati povratne informacije o događajima i aktivnostima u studentskom domu, kao što su teretana, turniri, druženja. Povratne informacije sadrže ocjenu od 1 do 5 te opcionalni komentar koji obrazlaže danu ocjenu. Povratne informacije služe ostalim studentima kako bi znali što mogu očekivati od aktivnosti te organizatorima da prepoznaju i riješe navedene probleme.
F09 | Studentski oglasnik | Korisnici mogu pregledavati i objavljivati obavijesti u studentskom oglasniku. Oglasnik služi za dijeljenje informacija o događanjima i aktivnostima u studentskom domu (npr. druženja, turniri, zabave). Svaka objava može sadržavati naslov, opis, datum i vrijeme događaja te opcionalnu sliku. Ostali korisnici mogu komentirati objave i označiti interes za dolazak. 
F10 | Prijava kvara u prostorijama | Sustav će omogućiti studentima prijavu kvara u studentskim učionicama i teretani. Svaka prijava kvara će sadržavati podatke o lokaciji, opisu problema te opcionalno fotografiju kvara. 
F11 | Upravljanje prijavljenim kvarovima | Sustav će omogućiti osoblju prikaz svih kvarova koje su studenti prijavili u studentskim učionicama i teretani. Osoblje mora imati uvid u sve prijavljene kvarove kako bi moglo pravovremeno reagirati i planirati popravke ili intervencije. Sustav će omogućiti filtriranje kvarova (po lokaciji, datumu prijave i vrsti problema), te vođenje evidencije (o učestalosti i vrsti kvarova). 
F12 | Prikaz tjednog jelovnika |Korisnik ima mogućnost pregledati tjedni jelovnik studentske menze koji se automatski dohvaća s web stranice (link : https://www.scvz.unizg.hr/jelovnik-varazdin/ ). Sustav preuzima i prikazuje ažurne informacije o dnevnoj ponudi ručka i večere, uključujući standardna i vegetarijanska jela, kao i dodatnu ponudu jela po narudžbi. Jelovnik se dinamički ažurira pri svakom otvaranju, čime se korisniku osigurava pristup najnovijim podacima dostupnima online.

## Tehnologije
#### Tehnologije:
 - WPF (.NET 8)
 - ASP.NET (.NET 9)
 - Microsoft SQL Server baza podataka

## Podaci za pristup
Email: obican@rpp.com lozinka: RPP_dev_test

Email: osoblje@rpp.com lozinka: RPP_dev_test

## Upute za lokalno pokretanje servera
### Preduvjeti
- Docker
- SMTP pristup (Gmail, obavezna autentifikacija).

1. Konfiguracija .env
   
Dodajte .env u root projekta sa sljedećim ključevima i vašim vrijednostima:

```
# SMTP (obavezno)
SMTP_USERNAME=
SMTP_PASSWORD=
SMTP_FROM_EMAIL=
SMTP_FROM_NAME=Campus4U


# Veza na bazu
CONNECTION_STRING=
```

2. Izmjena Caddyfile za lokalni test
   
Za lokalni test promijeni Caddyfile u:
```
:80 {
  reverse_proxy api:8080
}
```

Time API radi na http://localhost/.

3. Izmjena Campus4U\appsettings.json (klijent)

`"Api": { "BaseUrl": "http://localhost/" }`

4. Pokretanje preko Dockera
   
U rootu projekta:
`docker compose up --build`

Zaustavljanje:
`docker compose down`

# Campus4U

## Projektni tim

Ime i prezime | E-mail adresa (FOI) | JMBAG | GitHub korisničko ime
------------  | ------------------- | ----- | ---------------------
Nikola Kihas | nkihas23@student.foi.hr | 0016165847 | nkihas23
Tin Posavec | tposavec23@student.foi.hr | 0016165441 | tposavec23
Marko Mišić | mmisic23@student.foi.hr | 0016165901 | mmisic23 
Luka Kanjir | lkanjir23@student.foi.hr | 0016164491 | lkanjir23

## Opis domene
Aplikacija Campus4U usmjerena je na organizaciju života studenata koji borave u studentskom domu. Aplikacija ima cilj optimizirati svakodnevne aktivnosti studenata kao što su lakši pristup svim mogućnostima i prostorijama koje studentski kampus nudi, povećanje učinkovitosti korištenja zajedničkih prostora te unaprjeđenje suradnje među studentima. Aplikacija će studentima također ponuditi opciju pregleda dnevnog menija u studentskom restoranu. Aplikacija predstavlja desktop varijantu za mobilnu aplikaciju istog naziva koja se razvija za predmet RAMPU.

## Specifikacija projekta

Oznaka | Naziv | Kratki opis | Odgovorni član tima
------ | ----- | ----------- | -------------------
F01 | Registracija i prijava korisnika | Sustav omogućuje registraciju i prijavu putem korisničkog imena i lozinke. Na temelju uloge korisnika, sustav otvara različite prikaze i funkcionalnosti nakon uspješne prijave. U slučaju da račun korisnika ne postoji sustav korisniku nudi registraciju.| Luka Kanjir
F02 | Prikaz objekata i prostora | Korisnik ima mogućnost pregledavanja dostupnih učionica i teretane u studentskom domu. Prostore može pretraživati prema nazivu ili vrsti (učionica/teretana). Sustav prikazuje osnovne informacije o svakom prostoru, uključujući kapacitet i opremljenost. | Marko Mišić
F03 | Rezervacija prostorije | Korisnik odabire željenu učionicu ili teretanu te termin (datum i vrijeme) za koji želi kreirati rezervaciju. Korisnik ne može rezervirati prostoriju ako već postoji rezervacija za odabrano vrijeme. Ako je termin slobodan, korisnik može potvrditi rezervaciju, nakon čega ga aplikacija obavještava da je termin uspješno rezerviran. U tom trenutku, odabrani termin postaje nedostupan drugim studentima, a sustav automatski ažurira zauzetost prostora u stvarnom vremenu. | Marko Mišić
F04 | Upravljanje rezervacijama | Korisnik može pristupiti popisu svojih aktivnih i prethodnih rezervacija. Odabirom neke buduće rezervacije, može izmijeniti datum ili vrijeme ako je novi termin slobodan ili u potpunosti otkazati rezervaciju prije početka termina. Nakon što korisnik izvrši izmjenu ili otkazivanje, sustav ga obavještava o uspješnoj promjeni te ažurira status prostora u stvarnom vremenu. | Marko Mišić
F05 | Slanje obavijesti putem e-maila | Sustav automatski šalje obavijesti korisnicima o nadolazećim rezervacijama, promjenama statusa prostora ili prijavama kvarova te drugim važnim događajima. Sustav generira poruke koje uključuju relevantne informacije, kao što su naziv prostora, datum i vrijeme rezervacije, status te promjene termina. | Luka Kanjir
F06 | Upravljanje profilom korisnika | Korisnik može pregledavati i mijenjati podatke vlastitog profila te brisati neželjene neobavezne podatke. Podaci koji će biti prikazani su: korisničko ime, ime, prezime, broj sobe, korisnička slika, e-mail i broj telefona. Uz to korisnik može promijeniti lozinku profila. Obavezni podaci su podaci koji se unose kod registracije profila, a to su: korisničko ime i lozinka. | Nikola Kihas
F07 | Upravljanje favoritima | Korisnik može izabrati prostorije, događaje i aktivnosti kao favorite. Time će korisnik primati vezane obavijesti. Uz to, korisniku će se u posebnoj sekciji prikazati svi favoriti kako bi im čim prije mogao pristupiti. | Nikola Kihas
F08 | Ocjenjivanje i davanje povratnih informacija | Korisnici mogu davati povratne informacije o događajima i aktivnostima u studentskom domu, kao što su teretana, turniri, druženja. Povratne informacije sadrže ocjenu od 1 do 5 te opcionalni komentar koji obrazlaže danu ocjenu. Povratne informacije služe ostalim studentima kako bi znali što mogu očekivati od aktivnosti te organizatorima da prepoznaju i riješe navedene probleme. | Nikola Kihas
F09 | Studentski oglasnik | Korisnici mogu pregledavati i objavljivati obavijesti u studentskom oglasniku. Oglasnik služi za dijeljenje informacija o događanjima i aktivnostima u studentskom domu (npr. druženja, turniri, zabave). Svaka objava može sadržavati naslov, opis, datum i vrijeme događaja te opcionalnu sliku. Ostali korisnici mogu komentirati objave i označiti interes za dolazak. | Luka Kanjir
F10 | Prijava kvara u prostorijama | Sustav će omogućiti studentima prijavu kvara u studentskim učionicama i teretani. Svaka prijava kvara će sadržavati podatke o lokaciji, opisu problema te opcionalno fotografiju kvara. | Tin Posavec
F11 | Upravljanje prijavljenim kvarovima | Sustav će omogućiti osoblju prikaz svih kvarova koje su studenti prijavili u studentskim učionicama i teretani. Osoblje mora imati uvid u sve prijavljene kvarove kako bi moglo pravovremeno reagirati i planirati popravke ili intervencije. Sustav će omogućiti filtriranje kvarova (po lokaciji, datumu prijave i vrsti problema), te vođenje evidencije (o učestalosti i vrsti kvarova). | Tin Posavec
F12 | Prikaz tjednog jelovnika |Korisnik ima mogućnost pregledati tjedni jelovnik studentske menze koji se automatski dohvaća s web stranice (link : https://www.scvz.unizg.hr/jelovnik-varazdin/ ). Sustav preuzima i prikazuje ažurne informacije o dnevnoj ponudi ručka i večere, uključujući standardna i vegetarijanska jela, kao i dodatnu ponudu jela po narudžbi. Jelovnik se dinamički ažurira pri svakom otvaranju, čime se korisniku osigurava pristup najnovijim podacima dostupnima online. | Tin Posavec

### Skice
**Rezervacija prostorije**

<img width="1366" height="768" alt="Campus4U" src="https://github.com/user-attachments/assets/87919d58-8ef5-4e0b-9f06-cd285db3439d" />

**Dnevni jelovnik**

![desktop dnevni jelovnik - skica](https://github.com/user-attachments/assets/6d91ea33-60eb-4b22-9b08-989715b07237)

## Tehnologije i oprema
#### Tehnologije:
 - .NET Core
 - WPF 
 - Microsoft SQL Server baza podataka

#### Oprema:
 - Microsoft Visual Studio
 - Microsoft SQL Server Management Studio
 - git 
 - GitHub
 - SMTP poslužitelj

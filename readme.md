# Bezpieczny sieciowy menadżer haseł (Safe network password manager)

## Opis pracy
Celem pracy inżynierskiej jest zaprojektowanie i implementacja sieciowego menadżera haseł. Będzie to rozwiązanie typu klient-serwer, gdzie w części serwerowej przechowywane będą w bezpieczny sposób hasła i inne dane użytkowników, do których będą oni mieli dostęp poprzez aplikację kliencką. Rozważane jest zastosowanie silnej kryptografii także odpornej na kryptoanalizę kwantową.

Do komunikacji serwera z klientem posłuży rest api, a dane klientów będą także zaszyfrowane aby zminimalizować ich użyteczność w wypadku wycieku bazy danych. Aplikacja kliencka natomiast umożliwi tworzenie haseł (wraz z monitorowaniem poziomu bezpieczeństwa haseł), bezpieczne zarządzanie bazą haseł na serwerze oraz transparentne wykorzystanie menadżera w systemie Windows.

## Linki
* [Analiza ryzyka, stos technologiczny, szczegóły rozwiązania, opis metodyki pracy](https://docs.google.com/document/d/1D7LO9jubwGmLdtOAzCRFRyWriMip_gY4MTGMw9aZfBM/edit)
* [Arkusz metodyki pracy](https://docs.google.com/spreadsheets/d/18MJeDtFubeqbaqV3oKCtUYLAT2U9nz_t478Xu4LpVXE/edit#gid=0)
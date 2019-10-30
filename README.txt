Instrukcja aplikacji do rysowania wielok¹tów

S¹ dwa tryby: rysowania i "poruszania". Domyœlnie w³¹czony poruszania.

Oznaczenia relacji: kwadrat z dwoma kreskami- równoleg³oœæ, kwadrat bez kresek- równa d³ugoœæ. Max 6 relacji.

Aby poruszyæ wierzcho³kiem/krawêdzi¹ nale¿y na niego klikn¹æ myszk¹ i (bez wciœniêtego przycisku) poruszaæ myszk¹. Aby puœciæ krawêdŸ/wierzcho³ek nale¿y klikn¹æ myszk¹ jeszcze raz.

Aby na³o¿yæ relacjê nale¿y klikn¹æ na przycisk "Set equal length" lub "Set parallel", klikn¹æ na pierwsz¹ wybran¹ krawêdŸ, a potem na drug¹.
Aby usun¹æ relacjê nale¿y klikn¹æ przycisk "Remove Relation" i klikn¹æ na jedn¹ z krawêdzi z pary, której relacjê chce siê usun¹æ. 




W klasie Drawer znajduj¹ siê metody odpowiedzialne za rysowanie po bimapie.

W klasie Polygon znajduje siê logika nak³adania i zachowywania relacji.

W MainViewModel obs³ugiwane s¹ wszystkie akcje u¿ytkownika. 




Opis algorytmu: 

Utrzymywanie równoleg³oœci:
Gry poruszany jest jeden z wierzcho³ków krawêdzi o tej relacji, to druga krawêdŸ zmieniana jest tak, aby krawêdzie wci¹¿ by³y równoleg³e. 
Na podstawie tangensa k¹ta nachylenia pierwszej krawêdzi, odpowiednio przestawiany jest wierzcho³ek drugiej.

Utrzymywanie równej d³ugoœci:
Gry poruszany jest jeden z wierzcho³ków krawêdzi o tej relacji, to druga krawêdŸ zmieniana jest tak, aby krawêdzie wci¹¿ by³y równej d³ugoœci. 
Na podstawie d³ugoœci pierwszej krawêdzi, odpowiednio przestawiany jest wierzcho³ek drugiej, tak aby krawêdŸ by³a na tej samej prostej co wczeœniej. 
(Przez operacje na liczbach ca³kowitych, prosta moze byæ trochê przechylana)

Gdy wierzcho³ek drugiej krawêdzi relacji jest przestawiany przez algorytm, to sprawdzane jest czy mo¿e byæ tam ustawiony:
Jeœli druga krawêdŸ wychodz¹ca z tego wierzcho³ka nie ma relacji, to nie ma ¿adnych przeszkód.
Jeœli druga krawêdŸ ma relacjê, to wywo³ywany jest algorytm utrzymywania tej relacji i sprawdzane jest czy mo¿e byæ przestawiony i tak dalej...
Jeœli algorytm przejdzie dooko³a wielok¹ta lub zostanie przekroczona maksymalna liczba prób (3 * liczba wierzcho³ków wielok¹ta) to zwracany jest fa³sz i nie mo¿na przestawiæ w ten sposób.


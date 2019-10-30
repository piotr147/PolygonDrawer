Instrukcja aplikacji do rysowania wielok�t�w

S� dwa tryby: rysowania i "poruszania". Domy�lnie w��czony poruszania.

Oznaczenia relacji: kwadrat z dwoma kreskami- r�wnoleg�o��, kwadrat bez kresek- r�wna d�ugo��. Max 6 relacji.

Aby poruszy� wierzcho�kiem/kraw�dzi� nale�y na niego klikn�� myszk� i (bez wci�ni�tego przycisku) porusza� myszk�. Aby pu�ci� kraw�d�/wierzcho�ek nale�y klikn�� myszk� jeszcze raz.

Aby na�o�y� relacj� nale�y klikn�� na przycisk "Set equal length" lub "Set parallel", klikn�� na pierwsz� wybran� kraw�d�, a potem na drug�.
Aby usun�� relacj� nale�y klikn�� przycisk "Remove Relation" i klikn�� na jedn� z kraw�dzi z pary, kt�rej relacj� chce si� usun��. 




W klasie Drawer znajduj� si� metody odpowiedzialne za rysowanie po bimapie.

W klasie Polygon znajduje si� logika nak�adania i zachowywania relacji.

W MainViewModel obs�ugiwane s� wszystkie akcje u�ytkownika. 




Opis algorytmu: 

Utrzymywanie r�wnoleg�o�ci:
Gry poruszany jest jeden z wierzcho�k�w kraw�dzi o tej relacji, to druga kraw�d� zmieniana jest tak, aby kraw�dzie wci�� by�y r�wnoleg�e. 
Na podstawie tangensa k�ta nachylenia pierwszej kraw�dzi, odpowiednio przestawiany jest wierzcho�ek drugiej.

Utrzymywanie r�wnej d�ugo�ci:
Gry poruszany jest jeden z wierzcho�k�w kraw�dzi o tej relacji, to druga kraw�d� zmieniana jest tak, aby kraw�dzie wci�� by�y r�wnej d�ugo�ci. 
Na podstawie d�ugo�ci pierwszej kraw�dzi, odpowiednio przestawiany jest wierzcho�ek drugiej, tak aby kraw�d� by�a na tej samej prostej co wcze�niej. 
(Przez operacje na liczbach ca�kowitych, prosta moze by� troch� przechylana)

Gdy wierzcho�ek drugiej kraw�dzi relacji jest przestawiany przez algorytm, to sprawdzane jest czy mo�e by� tam ustawiony:
Je�li druga kraw�d� wychodz�ca z tego wierzcho�ka nie ma relacji, to nie ma �adnych przeszk�d.
Je�li druga kraw�d� ma relacj�, to wywo�ywany jest algorytm utrzymywania tej relacji i sprawdzane jest czy mo�e by� przestawiony i tak dalej...
Je�li algorytm przejdzie dooko�a wielok�ta lub zostanie przekroczona maksymalna liczba pr�b (3 * liczba wierzcho�k�w wielok�ta) to zwracany jest fa�sz i nie mo�na przestawi� w ten spos�b.


# eMTe
promenna cislo = 4096;
promenna root = 1;
promenna pocet = 0;
nactiInt pocet;
promenna i = 0;
pro i=0 do pocet delej
  vypis root;
  root = (cislo/root+root)/2;
konec;
vypis root;

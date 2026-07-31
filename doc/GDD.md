# GAME DESIGN DOCUMENT (GDD)
# Lendas do Quintal: O Sumiço da Vovó

---

# 1. Visão Geral

## Nome do Jogo
**Lendas do Quintal: O Sumiço da Vovó**

## Gênero
Plataforma 2D de aventura, mistério e combate leve em pixel art.

## Plataforma Inicial
- PC
- Android

## Plataformas Futuras
- Steam
- Nintendo Switch
- Consoles

## Engine
Unity 2D (C#)

## Público-Alvo
- Crianças, adolescentes e adultos
- Jogadores nostálgicos de jogos 2D
- Público brasileiro
- Fãs de folclore, mistério leve e pixel art

## Diferencial
Um jogo de plataforma com identidade brasileira, misturando casa de avó, quintal, interior, lendas do folclore e uma aventura investigativa acessível.

---

# 2. Conceito

Um neto ou uma neta vai passar o fim de semana na casa da avó no interior.

Na primeira noite, a avó conta histórias sobre criaturas do folclore brasileiro. Ela fala do Saci, da Iara, do Curupira e de outras lendas como se fossem mais do que simples histórias.

Durante a madrugada, a criança acorda e percebe que algo aconteceu:
- a casa está bagunçada;
- móveis saíram do lugar;
- objetos importantes sumiram;
- marcas estranhas aparecem no chão;
- um vento forte parece ter atravessado a casa;
- a avó desapareceu.

A partir daí, o jogador explora a casa, o quintal e os arredores para encontrar pistas, resolver pequenos desafios e descobrir o que aconteceu.

O mistério leva a criança a perceber que as lendas são reais e que a avó guardava um segredo antigo: ela ajudava a manter o equilíbrio entre o mundo humano e o mundo das lendas.

---

# 3. Tom e Direção Artística

## Estilo Visual
- Pixel art 16-bit
- Câmera lateral 2D
- Animações expressivas e legíveis
- Cenários com camadas de parallax
- Ambientes acolhedores com mistério leve

## Referências Visuais e de Clima
- A Link to the Past, pelo senso de aventura e mistério
- Monster Boy, pela exploração 2D colorida
- Shovel Knight, pela clareza de plataforma e combate
- Stardew Valley, pela atmosfera rural acolhedora

## Paleta de Cores
- Azul noturno
- Verde floresta
- Amarelo quente de casa iluminada
- Vermelho vivo em detalhes folclóricos
- Tons terrosos para quintal, madeira e interior

## Atmosfera
- Aventura
- Mistério familiar
- Humor leve
- Folclore brasileiro
- Nostalgia retrô
- Sensação de fim de semana no interior

---

# 4. Gameplay

## Estrutura Base
O jogador explora fases laterais com plataformas, obstáculos, inimigos, itens e pistas.

Cada área combina:
- travessia de plataformas;
- exploração;
- coleta de pistas;
- pequenos puzzles;
- combate simples;
- encontros com criaturas folclóricas.

## Movimento
- andar;
- correr;
- pular;
- cair em plataformas;
- subir escadas, cipós ou raízes;
- interagir com objetos;
- empurrar ou ativar elementos simples do cenário.

## Ações do Player
- andar;
- correr;
- pular;
- ataque básico;
- esquiva curta ou rolamento;
- interagir;
- usar item especial;
- examinar pistas.

## Combate
O combate deve ser simples e responsivo. Ele existe para criar tensão e variedade, mas não deve dominar a experiência.

### Ataque Normal
Ataque curto com objeto improvisado, como estilingue, guarda-chuva, lanterna ou brinquedo.

### Especial
Usa energia e varia conforme o personagem ou amuleto equipado.

### Esquiva
Movimento curto para evitar ataques e atravessar perigos pequenos.

---

# 5. Personagem Jogável

O jogador escolhe ou controla um neto ou uma neta.

## Neto
- mais resistente;
- ataque um pouco mais forte;
- pulo comum;
- especial baseado em redemoinho de folhas.

## Neta
- mais ágil;
- recuperação mais rápida;
- pulo um pouco mais responsivo;
- especial baseado em luz de amuleto.

## Observação de Escopo
Para o MVP, apenas um personagem jogável é necessário. A escolha entre neto e neta pode entrar depois, quando a base do jogo estiver sólida.

---

# 6. HUD

## Elementos da Interface
- Barra de vida;
- Barra de energia especial;
- Contador de pistas encontradas;
- Item equipado;
- Objetivo atual;
- Vidas ou tentativas, se necessário.

## Estilo
Pixel art retrô, discreto e fácil de ler.

---

# 7. Progressão

## Estrutura
O jogo será dividido em áreas conectadas por narrativa.

Cada fase possui:
- tema visual próprio;
- pistas sobre o desaparecimento da avó;
- obstáculos de plataforma;
- inimigos comuns;
- desafio especial;
- encontro com uma lenda.

---

# 8. Fases

# Fase 1 - A Casa e o Quintal

## Cenário
- quarto da criança;
- sala da avó;
- cozinha;
- varanda;
- quintal;
- galinheiro;
- poço;
- entrada da mata.

## Objetivo Narrativo
Descobrir que a avó desapareceu e encontrar as primeiras pistas.

## Pistas
- carta incompleta da avó;
- pegadas pequenas perto da janela;
- cachimbo ou gorro vermelho preso em um galho;
- folhas girando sem vento;
- objeto da avó levado para o quintal.

## Inimigos
- brinquedos encantados;
- galinhas assustadas ou possuídas;
- sombras pequenas;
- redemoinhos fracos.

## Encontro Principal
Saci-Pererê.

### Habilidades
- teleporte curto;
- redemoinho;
- roubo temporário de item;
- provocações e sumiços rápidos.

### Arena
Clareira pequena no fundo do quintal ou entrada da mata.

---

# Fase 2 - O Rio das Vozes

## Cenário
- mata alagada;
- ponte quebrada;
- pedras escorregadias;
- margem do rio;
- troncos flutuantes.

## Objetivo Narrativo
Seguir uma pista deixada pela avó e atravessar o rio encantado.

## Inimigos
- peixes monstruosos;
- espíritos d'água;
- raízes vivas;
- bolhas encantadas.

## Encontro Principal
Iara.

### Habilidades
- canto hipnótico;
- bolhas explosivas;
- ondas d'água;
- mudança temporária de direção do movimento.

---

# Fase 3 - A Vila Esquecida

## Cenário
- vila abandonada;
- ruas escuras;
- casas fechadas;
- cemitério;
- praça antiga.

## Objetivo Narrativo
Descobrir que outras pessoas esqueceram as lendas e alimentaram a entidade.

## Inimigos
- almas perdidas;
- espantalhos vivos;
- sombras de moradores;
- cães sombrios.

## Encontro Principal
Curupira e Lobisomem.

### Curupira
- cria ilusões;
- deixa pegadas falsas;
- altera caminhos da fase.

### Lobisomem
- corre em linha reta;
- salta entre plataformas;
- ataca com investidas rápidas.

---

# Fase 4 - O Mundo das Lendas

## Cenário
- dimensão espiritual;
- fragmentos da casa, quintal, rio e vila;
- plataformas distorcidas;
- céu estranho;
- objetos flutuantes.

## Objetivo Narrativo
Encontrar a avó e restaurar o equilíbrio entre os mundos.

## Inimigos
Versões corrompidas dos inimigos anteriores.

## Boss Final
A Entidade.

### Conceito
A Entidade é formada pelo medo coletivo, pelo esquecimento das histórias e pela energia das lendas corrompidas.

### Poderes
- redemoinho do Saci;
- canto da Iara;
- ilusões do Curupira;
- velocidade do Lobisomem.

---

# 9. História e Narrativa

## Estrutura Narrativa
A narrativa é contada através de:
- diálogos curtos;
- cenas pixel art;
- cartas da avó;
- objetos interativos;
- comentários da criança;
- pistas espalhadas pelo cenário.

## Mistério Central
O jogador precisa descobrir:
- onde a avó está;
- por que as lendas apareceram;
- qual era o papel da avó;
- como impedir que a Entidade atravesse para o mundo humano.

## Plot Twist
A avó não foi apenas vítima. Ela era uma guardiã das lendas e tentava impedir que uma força criada pelo esquecimento e pelo medo corrompesse o mundo das histórias.

---

# 10. Sistema de Itens

## Coletáveis
- doces;
- frutas;
- ervas;
- brinquedos antigos;
- cartas;
- amuletos;
- pedaços de mapa;
- lembranças da avó.

## Funções
- recuperar vida;
- recuperar energia;
- desbloquear passagens;
- revelar pistas;
- ativar habilidades especiais.

---

# 11. Sistema de Pistas

## Objetivo
Dar estrutura investigativa ao jogo sem quebrar o ritmo de plataforma.

## Exemplos
- examinar marcas no chão;
- encontrar cartas da avó;
- comparar objetos fora do lugar;
- seguir rastros;
- ativar memórias em objetos antigos.

## Uso no Gameplay
Algumas pistas são obrigatórias para avançar. Outras desbloqueiam diálogos, segredos ou áreas opcionais.

---

# 12. Sistema de Especiais

## Energia Especial
Preenchida ao:
- coletar itens;
- derrotar inimigos;
- encontrar pistas;
- evitar dano com precisão.

## Especiais
### Neto
Tempestade de folhas.

### Neta
Explosão luminosa de amuleto.

---

# 13. Áudio

## Trilha Sonora
Mistura de:
- chiptune;
- viola;
- flauta;
- percussão brasileira;
- sons regionais.

## Atmosfera Sonora
- grilos;
- vento;
- folhas;
- madeira rangendo;
- água corrente;
- sussurros;
- sons da noite no interior.

---

# 14. Efeitos Visuais

## Efeitos
- folhas voando;
- fumaça;
- brilho mágico;
- partículas pixeladas;
- vento do Saci;
- água encantada;
- pegadas brilhantes.

## Impacto
Os efeitos devem deixar ações e pistas claras, mantendo o visual retrô.

---

# 15. Sistema de Inimigos

## IA Base
- patrulhar;
- perseguir jogador;
- atacar ao aproximar;
- recuar;
- tomar dano;
- ficar atordoado;
- respeitar plataformas e bordas.

## Tipos
- patrulha simples;
- voador;
- perseguidor;
- inimigo de projétil;
- inimigo resistente;
- obstáculo vivo.

---

# 16. Sistema de Bosses

## Estrutura
Cada encontro principal possui:
- introdução narrativa;
- padrão de ataque;
- mudança de fase;
- fraqueza clara;
- recompensa narrativa;
- pista importante sobre a avó.

---

# 17. Menu Principal

## Opções
- Novo Jogo;
- Continuar;
- Configurações;
- Créditos;
- Sair.

## Fundo
Pixel art animado da casa da avó à noite, com luz acesa na varanda e folhas se movendo ao vento.

---

# 18. Configurações

## Opções
- volume;
- idioma;
- resolução;
- controles;
- tela cheia.

---

# 19. Salvamento

## Sistema
- autosave por área;
- checkpoints dentro das fases;
- slots de save em versão futura.

---

# 20. Direção Técnica

## Engine
Unity 2D

## Linguagem
C#

## Sistemas Principais
- controlador de plataforma;
- câmera lateral;
- combate simples;
- interação com objetos;
- sistema de pistas;
- checkpoints;
- HUD;
- inimigos básicos;
- boss simples.

## Estrutura de Pastas

```text
Assets/
  Art/
  Audio/
  Animations/
  Prefabs/
  Scenes/
  Scripts/
    Core/
    Player/
    Enemy/
    Systems/
    UI/
  UI/
```

---

# 21. MVP

## Objetivo
Criar um protótipo jogável contendo a primeira parte da Fase 1.

## Conteúdo do MVP
- personagem jogável;
- movimento lateral;
- pulo;
- plataforma sólida;
- colisão com chão e paredes;
- ataque básico;
- inimigo simples;
- interação com objetos;
- coleta de pistas;
- HUD simples;
- checkpoint;
- pequena sequência narrativa;
- encontro inicial com o Saci.

## Área do MVP
A casa da avó e o primeiro trecho do quintal.

## Loop do MVP
1. A criança acorda.
2. O jogador explora a casa bagunçada.
3. O jogador encontra uma pista da avó.
4. O jogador sai para o quintal.
5. O jogador atravessa plataformas simples e enfrenta inimigos fracos.
6. O jogador encontra o Saci.
7. O Saci foge, deixando uma nova pista para a próxima área.

---

# 22. Roadmap

## Etapa 1 - Base Jogável
- movimento lateral;
- pulo;
- câmera;
- colisão;
- uma sala/cena simples.

## Etapa 2 - Interação e Mistério
- objetos examináveis;
- pistas;
- objetivo atual;
- diálogos curtos.

## Etapa 3 - Combate
- ataque básico;
- inimigo simples;
- dano;
- vida;
- morte e respawn.

## Etapa 4 - Primeira Área
- casa;
- quintal;
- obstáculos;
- pickups;
- encontro com Saci.

## Etapa 5 - Polimento
- animações;
- efeitos;
- áudio;
- menu;
- build jogável.

---

# 23. Monetização

## Plataforma Principal
PC, com possibilidade futura de Steam.

## Possíveis Expansões
- novas lendas;
- capítulos extras;
- skins;
- modo coop em versão futura.

---

# 24. Diferencial do Projeto

- Folclore brasileiro;
- mistério familiar;
- casa de avó como ponto emocional;
- visual retrô moderno;
- plataforma 2D acessível;
- potencial para conteúdo em redes sociais;
- possibilidade de expansão para HQ, animação ou novos capítulos.

---

# 25. Visão Futura

Possibilidade de transformar o jogo em:
- franquia;
- animação;
- HQ;
- jogo mobile;
- sequência com novas lendas brasileiras;
- material educativo sobre folclore.

---

# Fim do Documento

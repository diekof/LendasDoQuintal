# MVP Unity - Lendas do Quintal

Este MVP implementa a primeira área jogável de **Lendas do Quintal: O Sumiço da Vovó** como um jogo de plataforma 2D com exploração, mistério e combate leve.

## Objetivo do MVP

Validar o núcleo do jogo:
- movimento lateral;
- pulo;
- exploração;
- interação com objetos;
- coleta de pistas;
- combate básico;
- primeira aparição do Saci.

## Conteúdo Jogável

- Player com movimento lateral.
- Corrida com `Shift`.
- Pulo com `Espaço`.
- Ataque normal com `J`.
- Esquiva ou rolamento curto com `K`.
- Interação com objetos usando `E`.
- Suporte inicial a controle: `A` pula, `X` ataca, `Y` interage, `LB/RB` corre e `Start` reinicia.
- Plataformas sólidas e obstáculos simples.
- Objetos examináveis dentro da casa da avó.
- Pistas coletáveis para avançar o objetivo.
- Inimigos fracos no quintal.
- Pickups de doce/erva para recuperar vida ou energia.
- Encontro inicial com o Saci.
- HUD com vida, energia, pistas e objetivo atual.
- Checkpoint simples.
- Reinício da fase com `R` após derrota ou fim do protótipo.

## Área do MVP

O MVP cobre:
- quarto onde a criança acorda;
- sala bagunçada da avó;
- cozinha ou varanda;
- primeiro trecho do quintal;
- entrada da mata.

## Loop Jogável

1. A criança acorda de madrugada.
2. O jogador encontra a casa bagunçada.
3. O jogador examina objetos e coleta a primeira pista.
4. A saída para o quintal é liberada.
5. O jogador atravessa plataformas simples no quintal.
6. O jogador enfrenta inimigos fracos.
7. O Saci aparece, provoca o jogador e foge.
8. Uma nova pista aponta para a próxima área.

## Como Gerar a Cena

No Unity, use o menu:

`Lendas do Quintal > Build MVP Scene`

O comando gera:
- `Assets/Scenes/LendasDoQuintal_MVP.unity`;
- sprites placeholder em `Assets/Generated`;
- prefabs em `Assets/Prefabs`;
- cena adicionada ao Build Settings.

## Arte Placeholder

O MVP pode usar arte placeholder gerada por script para validar gameplay. A troca por sprites definitivos deve preservar prefabs, colisores e componentes.

## Critérios de Sucesso

O MVP é considerado funcional quando:
- o player anda e pula com sensação responsiva;
- a câmera acompanha o jogador sem travar;
- plataformas e colisões funcionam;
- o jogador consegue examinar objetos;
- pistas atualizam o objetivo;
- inimigos causam e recebem dano;
- o encontro com o Saci comunica o mistério principal;
- o protótipo pode ser concluído em poucos minutos.

# MVP Unity - Lendas do Quintal

Este MVP implementa a primeira area da Fase 1: o quintal amaldicoado, uma onda de inimigos e o mini boss Saci.

## Conteudo jogavel

- Player com movimento em 8 direcoes.
- Corrida com `Shift`.
- Ataque normal com `J` e combo simples.
- Esquiva com `K`, evitando dano durante o dash.
- Especial com `L`, consumindo barra de energia.
- Galinhas possuidas perseguindo e causando dano por contato.
- Pickups de doce/erva para recuperar vida e energia.
- Mini boss Saci com perseguicao, teleporte e redemoinhos.
- HUD com vida, energia, objetivo, vitoria e derrota.
- Reinicio da fase com `R` apos vitoria/derrota.

## Como gerar a cena

No Unity, use:

`Lendas do Quintal > Build MVP Scene`

O comando gera:

- `Assets/LendasDoQuintal.unity`
- sprites pixel placeholder em `Assets/Generated`
- prefabs em `Assets/Prefabs`
- cena adicionada ao Build Settings

## Observacao

O construtor usa arte placeholder gerada por script para validar gameplay. A troca por sprites definitivos pode ser feita mantendo os mesmos prefabs e componentes.

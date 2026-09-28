# Lendas do Quintal: O Sumiço da Vovó - Setup do Protótipo

Este documento descreve uma base técnica inicial para o protótipo Unity de plataforma 2D.

## 1. Estrutura de Pastas Sugerida

```text
Assets/
  Scenes/
  Scripts/
    Core/
      Health.cs
      DamageDealer.cs
    Player/
      PlayerPlatformMovement.cs
      PlayerCombat.cs
      PlayerInteraction.cs
    Enemy/
      EnemyPatrol.cs
      EnemyChase.cs
      EnemyCombat.cs
    Systems/
      ClueSystem.cs
      ObjectiveSystem.cs
      CheckpointSystem.cs
      GameFlowController.cs
    Camera/
      CameraFollow2D.cs
    UI/
      HealthBarUI.cs
      ObjectiveUI.cs
      ClueCounterUI.cs
  Prefabs/
  Art/
  Audio/
  Animations/
  UI/
```

## 2. Hierarquia da Cena

Exemplo para a primeira cena do MVP:

- `Main Camera`
- `GameFlow`
- `Canvas`
- `Environment`
- `Player`
- `Checkpoint_Start`
- `Interactables`
- `Clues`
- `Enemies`
- `SaciEncounter`

## 3. Configuração do Player

Objeto: `Player`

Componentes:
- `Rigidbody2D`
- `CapsuleCollider2D` ou `BoxCollider2D`
- `Health`
- `PlayerPlatformMovement`
- `PlayerCombat`
- `PlayerInteraction`
- `Animator`
- `SpriteRenderer`

Configuração importante:
- Tag: `Player`
- Layer: `Player`
- `Rigidbody2D`:
  - Body Type: Dynamic
  - Gravity Scale: valor maior que `0`
  - Freeze Rotation Z: `true`
- Collider ajustado ao corpo do personagem.
- Criar um filho chamado `GroundCheck`.
- Criar um filho chamado `AttackPoint`.
- Criar um filho chamado `InteractionPoint`.

## 4. Movimento de Plataforma

Inputs sugeridos:
- Movimento: `A/D` ou setas;
- Corrida: `Shift`;
- Pulo: `Espaço`;
- Ataque: `J`;
- Esquiva: `K`;
- Interação: `E`;
- Reiniciar: `R`.

Parâmetros iniciais:
- velocidade de caminhada;
- velocidade de corrida;
- força do pulo;
- checagem de chão;
- tolerância pequena para pulo logo após sair da plataforma;
- buffer curto para pulo antes de tocar o chão.

## 5. Configuração do Cenário

Objetos de chão e plataforma:
- `TilemapCollider2D` ou `BoxCollider2D`;
- Layer: `Ground`;
- se usar Tilemap, considerar `CompositeCollider2D`.

Elementos iniciais:
- piso da casa;
- móveis como obstáculos;
- janela ou porta interativa;
- plataformas no quintal;
- cerca;
- poço;
- galinheiro.

## 6. Configuração dos Interactables

Objetos examináveis devem ter:
- collider com `Is Trigger`;
- script de interação;
- texto curto de descrição;
- indicação se entrega pista ou apenas comentário.

Exemplos:
- carta rasgada da avó;
- cadeira caída;
- janela aberta;
- marcas de redemoinho;
- gorro vermelho preso no galho.

## 7. Sistema de Pistas

Scripts sugeridos:
- `ClueSystem`;
- `CluePickup`;
- `ObjectiveSystem`;
- `ObjectiveUI`.

Fluxo:
1. O jogador interage com um objeto.
2. O objeto registra uma pista.
3. O contador de pistas é atualizado.
4. O objetivo atual muda quando pistas obrigatórias são encontradas.
5. Uma porta, passagem ou encontro pode ser liberado.

## 8. Configuração de Inimigos

Objeto: `Enemy_Chicken` ou `Enemy_Shadow`

Componentes:
- `Rigidbody2D`
- `Collider2D`
- `Health`
- `EnemyPatrol`
- `EnemyCombat`
- `Animator`
- `SpriteRenderer`

Configuração importante:
- Tag: `Enemy`
- Layer: `Enemy`
- `Rigidbody2D`:
  - Body Type: Dynamic
  - Gravity Scale: maior que `0`
  - Freeze Rotation Z: `true`
- Inimigo deve respeitar chão, paredes e bordas.

## 9. Configuração da Câmera

Objeto: `Main Camera`

Componentes:
- `Camera`
- `AudioListener`
- `CameraFollow2D`

Configuração importante:
- `CameraFollow2D.target` pode buscar o player por tag;
- offset sugerido: `0, 1, -10`;
- suavização leve;
- limites opcionais da fase para não mostrar fora do cenário.

## 10. HUD

Elementos:
- barra de vida;
- barra de energia;
- contador de pistas;
- texto de objetivo atual;
- painel de diálogo curto;
- tela de derrota;
- tela de fim do protótipo.

## 11. Fluxo de Fase

Scripts sugeridos:
- `GameFlowController`;
- `CheckpointSystem`;
- `ObjectiveSystem`.

Fluxo esperado:
1. Cena inicia no quarto.
2. Objetivo: "Descubra o que aconteceu."
3. Jogador examina a casa.
4. Após a pista obrigatória, objetivo muda para "Vá até o quintal."
5. Jogador atravessa o quintal.
6. Encontro com Saci é ativado.
7. Saci foge.
8. Protótipo termina com nova pista.

## 12. Animator

Parâmetros sugeridos:
- `Speed` (Float)
- `IsGrounded` (Bool)
- `VerticalVelocity` (Float)
- `Attack` (Trigger)
- `Hurt` (Trigger)
- `Interact` (Trigger)

Estados mínimos:
- Idle;
- Run;
- Jump;
- Fall;
- Attack;
- Hurt;
- Interact.

## 13. Resultado Esperado

Com essa base:
- o player anda, corre e pula;
- o player interage com objetos;
- pistas atualizam o objetivo;
- inimigos patrulham e atacam;
- o player causa e recebe dano;
- a câmera segue suavemente;
- checkpoints funcionam;
- o MVP comunica a história do desaparecimento da avó.

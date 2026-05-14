# Lendas do Quintal: O Sumiço da Vovó - Setup do Protótipo

## 1) Estrutura de pastas sugerida
- `Assets/Scripts/Core/Health.cs`
- `Assets/Scripts/Player/PlayerMovement.cs`
- `Assets/Scripts/Player/PlayerCombat.cs`
- `Assets/Scripts/Enemy/EnemyChase.cs`
- `Assets/Scripts/Enemy/EnemyCombat.cs`
- `Assets/Scripts/Camera/CameraFollow.cs`

## 2) Hierarquia da cena (exemplo)
- `Main Camera`
- `GameManager` (opcional)
- `Environment`
- `Player`
- `Enemy_01`
- `Enemy_02` (opcional)

## 3) Configuração do Player
Objeto: `Player`

Componentes:
- `Rigidbody2D`
- `CapsuleCollider2D` (ou `BoxCollider2D`)
- `Health`
- `PlayerMovement`
- `PlayerCombat`
- `Animator` (no filho visual ou no próprio objeto)
- `SpriteRenderer` (no filho visual ou no próprio objeto)

Configuração importante:
- Tag: `Player`
- Layer: `Player`
- `Rigidbody2D`:
  - Body Type: Dynamic
  - Gravity Scale: `0`
  - Freeze Rotation Z: `true`
- Crie um filho chamado `AttackPoint` na frente do player.
- Em `PlayerCombat`:
  - Arraste `AttackPoint` para `attackPoint`
  - Defina `enemyLayer` para a layer dos inimigos

## 4) Configuração do Enemy
Objeto: `Enemy_01`

Componentes:
- `Rigidbody2D`
- `CapsuleCollider2D` (ou `BoxCollider2D`)
- `Health`
- `EnemyChase`
- `EnemyCombat`
- `Animator` (opcional)
- `SpriteRenderer`

Configuração importante:
- Tag: `Enemy` (opcional para organização)
- Layer: `Enemy`
- `Rigidbody2D`:
  - Body Type: Dynamic
  - Gravity Scale: `0`
  - Freeze Rotation Z: `true`
- `EnemyChase.target` pode ficar vazio (ele busca o Player por tag automaticamente)

## 5) Configuração da câmera
Objeto: `Main Camera`

Componentes:
- `Camera`
- `AudioListener`
- `CameraFollow`

Configuração importante:
- `CameraFollow.target` pode ficar vazio (busca Player por tag)
- Ajuste `offset` para `0, 0, -10`

## 6) Inputs
- Movimento: `WASD` ou setas (eixos padrão `Horizontal` e `Vertical`)
- Ataque: tecla `J`

## 7) Animator (extra)
Parâmetros sugeridos:
- `Speed` (Float)
- `MoveX` (Float)
- `MoveY` (Float)
- `Attack` (Trigger)

Uso básico:
- Idle <-> Walk com condição `Speed > 0.01`
- Transição para ataque via Trigger `Attack`

## 8) Resultado esperado
Com essa base:
- Player anda em 8 direções
- Player ataca com cooldown
- Inimigo persegue e para em distância mínima
- Inimigo causa dano por contato
- Ambos usam `Health`
- Inimigo morre ao zerar vida
- Câmera segue o player suavemente

## 9) Sistemas adicionais (onda + vitória/derrota + HUD)
Novos scripts:
- `Assets/Scripts/Systems/EnemySpawner.cs`
- `Assets/Scripts/Systems/GameFlowController.cs`
- `Assets/Scripts/UI/HealthBarUI.cs`

### Spawner de inimigos
1. Crie um objeto vazio `EnemySpawner`.
2. Adicione o componente `EnemySpawner`.
3. Arraste o prefab do inimigo em `enemyPrefab`.
4. Crie 3 a 6 objetos vazios como pontos de spawn (ex.: `SpawnPoint_A`, `SpawnPoint_B`...).
5. Arraste todos para a lista `spawnPoints`.
6. Ajuste:
   - `totalToSpawn` (ex.: 6)
   - `maxAliveEnemies` (ex.: 2)
   - `spawnInterval` (ex.: 1.5)

### Fluxo de fase (vitória / derrota)
1. Crie um objeto vazio `GameFlow`.
2. Adicione `GameFlowController`.
3. Arraste:
   - `playerHealth` = componente `Health` do player
   - `enemySpawner` = objeto `EnemySpawner`
   - `victoryPanel` e `gameOverPanel` (UI opcionais)
4. Durante fim de partida, tecla `R` reinicia a cena.

### HUD de vida do jogador
1. Crie um `Canvas` (Screen Space - Overlay).
2. Dentro do Canvas, crie `UI > Slider` chamado `PlayerHealthBar`.
3. Adicione `HealthBarUI` no mesmo objeto do slider.
4. Em `HealthBarUI`:
   - `targetHealth` = `Health` do player
   - `healthSlider` = slider atual
5. Opcional: para barra de inimigo, duplique o slider e troque o `targetHealth`.

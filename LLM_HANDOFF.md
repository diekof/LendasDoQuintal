# Handoff - Lendas do Quintal

Este arquivo resume o estado do projeto para outra LLM continuar o trabalho sem depender do histórico completo da conversa.

## Projeto

Repositório local:

```text
D:\dev\LendasDoQuintal
```

Branch principal de trabalho:

```text
codex/unity-poc
```

Remoto:

```text
https://github.com/diekof/LendasDoQuintal.git
```

## Visão do Jogo

O projeto começou como uma ideia de beat 'em up, mas foi redirecionado para:

```text
Plataforma 2D de aventura, mistério e combate leve em pixel art.
```

Premissa:

Um neto ou uma neta passa o fim de semana na casa da avó no interior. Durante a madrugada, a avó desaparece, a casa fica bagunçada e pistas indicam que lendas do folclore brasileiro são reais. O jogador explora a casa, o quintal e áreas próximas para resolver o mistério.

Tom visual:

- noite azul;
- luz quente de casa/lanterna;
- madeira;
- verde vivo de quintal/floresta;
- contorno escuro;
- pixel art/cartoon de alta leitura;
- clima de mistério familiar e aventura.

## Documentação Atualizada

Arquivos importantes:

```text
doc/GDD.md
doc/MVP_UNITY.md
doc/SETUP_UNITY_PROTO.md
README.md
Assets/Art/STYLE_GUIDE.md
```

Esses docs já refletem a direção de plataforma 2D.

## Como Rodar no Unity

No Unity Editor, abrir o projeto:

```text
D:\dev\LendasDoQuintal
```

Depois usar o menu superior:

```text
Lendas do Quintal > Build MVP Scene
```

Isso gera/atualiza a cena:

```text
Assets/Scenes/LendasDoQuintal_MVP.unity
```

Depois apertar Play.

Observação:

O comando acima é um menu do Unity, não um comando PowerShell.

## Como Gerar Demo/Build

No Unity:

```text
File > Build Profiles
```

ou:

```text
File > Build Settings
```

Selecionar Windows, garantir que `LendasDoQuintal_MVP` está em `Scenes in Build`, e gerar em:

```text
D:\dev\LendasDoQuintal\Builds\Windows
```

## POC Atual

A POC contém:

- player com movimento lateral;
- corrida;
- pulo;
- ataque;
- interação;
- sistema simples de pistas;
- HUD;
- inimigo galinha;
- encontro com Saci;
- cena placeholder gerada por script;
- arte inicial do herói;
- walk cycle do herói;
- avatar da galinha demoníaca.

Controles:

```text
A/D ou setas: mover
Shift: correr
Espaço: pular
J: atacar
E: interagir
R: reiniciar depois de derrota/fim
```

Controle/gamepad:

```text
Analógico/direcional: mover
A: pular
X: atacar
Y: interagir
LB/RB: correr
Start: reiniciar
```

## Scripts Principais

```text
Assets/Editor/MvpSceneBuilder.cs
```

Gera a cena MVP via menu do Unity.

```text
Assets/Scripts/Player/PlayerPlatformMovement.cs
Assets/Scripts/Player/PlayerCombat.cs
Assets/Scripts/Player/PlayerInteraction.cs
Assets/Scripts/Player/PlayerSpriteAnimator.cs
```

Controlam movimento, ataque, interação e animação por troca de sprites.

```text
Assets/Scripts/Core/InputReader.cs
Assets/Scripts/Core/Health.cs
Assets/Scripts/Core/IInteractable.cs
```

Base de input, vida e interação.

```text
Assets/Scripts/Enemy/EnemyPatrol.cs
Assets/Scripts/Enemy/EnemyCombat.cs
```

Comportamento simples de inimigo.

```text
Assets/Scripts/Systems/ClueSystem.cs
Assets/Scripts/Systems/ObjectiveSystem.cs
Assets/Scripts/Systems/GameFlowController.cs
Assets/Scripts/Systems/InteractableObject.cs
Assets/Scripts/Systems/SaciEncounter.cs
```

Sistemas de pistas, objetivo, fluxo, objetos interativos e fim da demo.

## Arte Gerada

Concept/mood art:

```text
Assets/Art/Concept/lendas_quintal_key_art_reference.png
```

Herói idle:

```text
Assets/Art/Characters/Hero/hero_idle_side_64.png
```

Walk cycle:

```text
Assets/Art/Characters/Hero/Walk/hero_walk_side_00.png
Assets/Art/Characters/Hero/Walk/hero_walk_side_01.png
Assets/Art/Characters/Hero/Walk/hero_walk_side_02.png
Assets/Art/Characters/Hero/Walk/hero_walk_side_03.png
Assets/Art/Characters/Hero/Walk/hero_walk_side_04.png
Assets/Art/Characters/Hero/Walk/hero_walk_side_05.png
Assets/Art/Characters/Hero/Walk/hero_walk_side_06.png
Assets/Art/Characters/Hero/Walk/hero_walk_side_07.png
```

Avatar inimigo:

```text
Assets/Art/Enemies/Chicken/demonic_chicken_avatar_128_final.png
```

## PixelLab

Foi configurado MCP do PixelLab no Codex local em:

```text
C:\Users\diego\.codex\config.toml
```

Não colocar token/chave no repositório.

PixelLab já foi usado para:

- herói side-view;
- walk cycle do herói;
- avatar da galinha demoníaca.

## Commits Relevantes na Branch

```text
8418a66 Add Unity platformer POC
45e35db Add initial visual direction assets
f78d1ac Fix Unity 6 scene builder compatibility
99f529b Add PixelLab hero sprite
25adc54 Add hero walk animation
4769acb Add demonic chicken enemy avatar
```

Pode haver commits posteriores se o usuário continuar mexendo no Unity.

## Atenção: Worktree Local Está Sujo

No momento em que este arquivo foi criado, o `git status` mostrava várias mudanças locais não commitadas, provavelmente geradas pelo Unity ou por trabalho posterior.

Não sobrescrever ou reverter sem revisar.

Exemplos vistos:

```text
Assets/Scenes/LendasDoQuintal_MVP.unity
ProjectSettings/*
Packages/*
Assets/Generated/*
Assets/Scripts/*
Assets/Art/Backgrounds/
Assets/Art/Characters/Hero/Attack/
Assets/Art/Characters/Hero/Jump/
Assets/Art/Effects/
Assets/Scripts/Systems/ClueMessageSystem.cs
Assets/Scripts/Systems/DemoFlowController.cs
Assets/Scripts/Systems/OneShotSpriteAnimation.cs
Assets/Scripts/UI/ClueMessageUI.cs
```

Antes de qualquer commit novo:

```powershell
git status --short
git diff --stat
```

Separar mudanças por escopo. Não usar `git add -A` sem revisar.

## Próximos Passos Sugeridos

1. Validar no Unity se o walk cycle está tocando ao andar.
2. Melhorar pulo e queda com sprites próprios.
3. Criar ataque animado do herói.
4. Integrar avatar/arte da galinha ao inimigo real ou UI.
5. Melhorar cenário da casa/quintal com tiles em vez de blocos grandes.
6. Criar build Windows da demo.
7. Abrir PR da branch `codex/unity-poc` para `main`.

## Observações Técnicas

O projeto está em Unity 6.6.

Em Unity 6, `Rigidbody2D.velocity` foi migrado para:

```csharp
rb.linearVelocity
```

Também houve correção de fonte builtin:

```csharp
Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")
```

`Arial.ttf` causava erro no Unity 6.


# Lendas do Quintal: O Sumiço da Vovó

POC em Unity 2D para um jogo de plataforma, aventura e mistério inspirado no folclore brasileiro.

## Estado Atual

O repositório contém:
- documentação de design em `doc/`;
- estrutura mínima de projeto Unity;
- scripts C# para a POC;
- gerador de cena placeholder pelo menu do Unity.
- guia visual e concept art de referência em `Assets/Art/`.
- fundo pixel art largo da fase 1, pronto para câmera 16:9 / 1920x1080.

## Como Testar a POC

Para a Fase 1 com percurso de pelo menos 3 minutos até o Saci, cinco pistas, quatro tipos de inimigos, checkpoints e árvore de habilidades, use **Lendas do Quintal > Build Phase 1 Scene** e rode `Assets/Scenes/LendasDoQuintal_Phase1.unity`. Veja `doc/PHASE1_IMPLEMENTATION.md` para regras, controles e limites do protótipo. `Tab` abre a árvore; `L` usa o especial desbloqueado.

1. Abra esta pasta no Unity Hub.
2. Use uma versão Unity 2D compatível com projetos C# e UGUI.
3. No editor, abra o menu:

```text
Lendas do Quintal > Build MVP Scene
```

4. Esse comando também recria os sprites placeholder com a paleta atual.
5. Abra ou rode a cena gerada:

```text
Assets/Scenes/LendasDoQuintal_MVP.unity
```

## Arte

- Guia visual: `Assets/Art/STYLE_GUIDE.md`
- Concept/mood art: `Assets/Art/Concept/lendas_quintal_key_art_reference.png`
- Fundo fase 1: `Assets/Art/Backgrounds/Phase1_Backyard_Background_Wide.png`
- Herói idle: `Assets/Art/Characters/Hero/hero_idle_side_64.png`
- Caminhada do herói: `Assets/Art/Characters/Hero/Walk/`
- Soco do herói: `Assets/Art/Characters/Hero/Attack/`
- Pulo do herói: `Assets/Art/Characters/Hero/Jump/`
- Rolagem do herói: `Assets/Art/Characters/Hero/Roll/`
- Impacto do soco: `Assets/Art/Effects/Impact/`

## Controles

- `A/D` ou setas: mover;
- `Shift`: correr;
- `Espaço`: pular;
- `J`: atacar;
- `K`: rolar/esquivar;
- `E`: interagir;
- `R`: reiniciar após derrota ou fim do protótipo.

## Controle

- Analógico esquerdo ou direcional: mover;
- `A`: pular;
- `X`: atacar;
- `B`: rolar/esquivar;
- `Y`: interagir;
- `LB/RB`: correr;
- `Start`: reiniciar após derrota ou fim do protótipo.

## Loop da POC

1. A criança acorda na casa da avó.
2. O jogador examina uma pista.
3. O objetivo muda para ir ao quintal.
4. O jogador atravessa plataformas simples.
5. Um inimigo patrulha o quintal.
6. O encontro com o Saci encerra o protótipo.

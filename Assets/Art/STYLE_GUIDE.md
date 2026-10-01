# Lendas do Quintal - Guia Visual Inicial

Este guia parte da imagem de referência enviada pelo usuário, usando-a apenas como direção de estilo: alto contraste, noite azul, luz quente, floresta viva, madeira e contornos fortes.

## Paleta Base

- `#070A12` - contorno quase preto;
- `#10182A` - fundo noturno;
- `#123B6D` - azul profundo;
- `#15AEFF` - destaque azul elétrico;
- `#FFB52D` - luz quente de janela/lanterna;
- `#CA601F` - telhado, terra quente e madeira iluminada;
- `#5F351E` - madeira escura;
- `#2B7532` - verde de folhagem;
- `#6FBE23` - verde vivo de destaque;
- `#C21E33` - vermelho do Saci e sinais mágicos;
- `#F4DFA4` - papel, placas e pistas.

## Direção

- Cenários noturnos devem misturar sombras azuladas com pontos fortes de luz laranja.
- Personagens e objetos importantes usam contorno escuro grosso para leitura rápida.
- Elementos mágicos podem usar vermelho, ciano ou magenta, mas com moderação.
- Madeira, placas, cercas, casa e árvore são elementos visuais importantes do jogo.
- A casa da avó deve parecer acolhedora mesmo quando o mistério estiver presente.

## Aplicação na POC

- `Assets/Editor/MvpSceneBuilder.cs` gera sprites placeholder seguindo esta paleta.
- `Assets/Art/Concept/lendas_quintal_key_art_reference.png` serve como referência visual do clima.
- `Assets/Art/Backgrounds/Phase1_Backyard_Background_Wide.png` é o fundo largo da fase 1, derivado do PixelLab para reduzir repetição durante o avanço da câmera.
- `Assets/Art/Characters/Hero/hero_idle_side_64.png` é o primeiro sprite real do herói gerado com PixelLab.
- `Assets/Art/Characters/Hero/Walk/` contém os frames de caminhada do herói gerados com PixelLab.
- `Assets/Art/Characters/Hero/Attack/` e `Assets/Art/Characters/Hero/Jump/` contêm os primeiros frames de soco e pulo.
- `Assets/Art/Effects/Impact/` contém o efeito de impacto do soco.
- A arte ainda é placeholder; o objetivo agora é validar direção, legibilidade e atmosfera.

# Fase 1 — implementação jogável

A POC original continua em `Assets/Scenes/LendasDoQuintal_MVP.unity`. A cena nova é `Assets/Scenes/LendasDoQuintal_Phase1.unity`. Para reconstruí-la: **Lendas do Quintal > Build Phase 1 Scene**. Salve alterações manuais de outra cena antes de executar o comando.

## Percurso e duração

Oito áreas conectadas: quarto, sala, cozinha, varanda, quintal/galinheiro, poço, entrada da mata e clareira. Cada trecho tem plataformas, uma fruta que recupera vida/energia e um checkpoint. As cinco pistas do GDD são obrigatórias e sinalizadas com `[E]`; o gorro exige subir nos galhos próximos ao poço.

Sete encantamentos dividem a progressão. Cada um exige limpar os inimigos e examinar a pista daquele trecho, quando houver. Os limites temporais são 25,7 / 51,4 / 77,1 / 102,9 / 128,6 / 154,3 / 180 segundos. Reforços surgem a cada seis segundos, com no máximo dois vivos, até cinco segundos antes da liberação. O cronômetro conta gameplay despausado; menus e árvore não contam. Não garante exatamente três minutos para qualquer jogador: mortes, combate e exploração podem prolongar o percurso. Esse controle de ritmo é uma primeira proposta, a calibrar em playtest.

Os sete trechos têm 140 unidades cada: a corrida contínua a 7 unidades/segundo ocuparia cerca de 140 segundos, deixando o restante da meta para encontros e investigação. Isso é uma estimativa de layout, não uma medição de uma partida humana.

Brinquedos encantados perseguem lentamente; galinhas anunciam projéteis; sombras são rápidas e frágeis; redemoinhos flutuam e causam dano por contato. Inimigos terrestres verificam chão à frente, permanecem no trecho e ficam brevemente atordoados ao tomar dano. A dificuldade altera velocidade/dano dos inimigos e dano do Saci.

## Saci

O encontro começa ao entrar na clareira após os sete trechos. A arena fecha durante a luta. O Saci tem 18 de vida e alterna redemoinho, teleporte curto e roubo do amuleto por três segundos (bloqueia ataque/especial; movimento e esquiva continuam). Avisos precedem ataques; a cor verde sinaliza a janela de vulnerabilidade. Com metade da vida, avisos e intervalos encurtam. A vitória revela a avó como guardiã e aponta para o Rio das Vozes.

## Árvore do herói — proposta inicial

O GDD não especificava nós ou custos. Esta implementação acrescenta uma proposta para o neto:

| Nó | Efeito | Pré-requisito | Custo |
|---|---|---|---|
| Coragem | +2 de vida máxima | Raiz | 1 |
| Punho firme | +1 de dano básico | Raiz | 1 |
| Passo leve | Intervalo reduzido de 0,65s para 0,56s; mantém 0,12s sem invulnerabilidade | Raiz | 1 |
| Tempestade de folhas | Especial: 2 de dano em raio de 2,7 | Punho firme | 1 |
| Raízes fortes | +2 de vida máxima | Coragem | 2 |
| Vendaval | Especial: 4 de dano em raio de 4 | Tempestade de folhas | 2 |

Começa com um ponto. A cada três unidades de experiência, ganha outro ponto: inimigo vale uma; pista inédita vale duas. O especial custa 40 de energia; inimigos devolvem 12, pistas 20 e frutas 25. Vida máxima extra não cura imediatamente. **Tab** ou **Back/Select** abre a árvore; clique ou teclas **1–6** compram nós. **L** ou clique do analógico esquerdo usa o especial. Navegação pelos nós via direcional do gamepad ainda não está implementada.

## Checkpoints e salvamento

A partida começa com **4 vidas**, separadas da saúde representada pelos corações. Cada morte consome uma vida: nas três primeiras, o herói retorna ao checkpoint, recupera saúde, recebe dois segundos de proteção e os projéteis são removidos. Na arena, reinicia o Saci. Ao morrer pela quarta vez, as vidas chegam a zero, o checkpoint salvo é invalidado e aparece a seleção de dificuldade. Pontos, pistas e habilidades permanecem nas tentativas restantes. Frutas recuperam saúde e energia, não vidas.

Antes de consumir a vida, uma animação de 2,4 segundos mostra o herói de pijama caminhando com uma caneca de leite quente e vapor. O contador então mostra a vida descontada por 0,7 segundo. Durante os 3,1 segundos da transição, o mundo, o cronômetro da fase, os controles e a física do herói ficam pausados; a animação usa tempo real. Só depois ocorre o respawn ou a seleção na quarta morte. Eventos repetidos de morte durante a transição não consomem vidas extras.

A arte foi criada no PixelLab com ferramentas de desenho/animacão exata, pois a conta informou que os créditos de geração por IA acabaram. Desenho `fc238879-9897-4812-a794-43076d9d6f1d`: oito quadros de 64×64, 120 ms por quadro, grade de três colunas. O rosto vem do sprite existente do herói. A folha está em `Assets/Resources/HeroDeath/pajama_walk.png`, e a receita do PixelLab acompanha o arquivo. Preview: `output/gameplay/hero-death-pajamas.gif`.

Cada entrada de área e cada respawn grava um checkpoint local em PlayerPrefs, com área, tempo, pistas, árvore e vidas restantes. **Continuar** restaura esse checkpoint sem devolver vidas consumidas; **Jogar** começa com quatro vidas. A dificuldade selecionada no menu vale também para Continuar. Não existem múltiplos slots ou salvamento em nuvem.

## Validação e limites

**Lendas do Quintal > Validate Phase 1 Gameplay** reconstrói a cena e testa em Play Mode. É um teste de regras com avanço controlado do relógio/posições; não substitui uma partida manual para validar diversão, legibilidade e duração real. Em batch, use `LendasDoQuintal.Editor.Phase1Validation.BuildAndValidate` sem `-quit`; o teste encerra o editor ao terminar e escreve `.codex-build/phase1-check/gameplay-validation.txt`.

**Lendas do Quintal > Validate Death Animation** testa quatro mortes com reprodução real da animação: quadros avançando durante pausa, contador descontado somente ao fim, três retornos ao checkpoint e seleção na quarta morte. Escreve `.codex-build/phase1-check/death-animation-validation.txt`. Em batch, use `LendasDoQuintal.Editor.DeathAnimationValidation.Run` sem `-quit`.

Os oito trechos receberam mobiliário, janelas, luminárias, galinheiro, poço, cercas, plantas, árvores e pedras, com marcos distribuídos ao longo do percurso. Dois atlases de 16 objetos foram gerados com imagegen; caminhos e prompts estão em `PHASE1_ART_PROMPTS.md`. A decoração fica atrás do herói e não adiciona obstáculos invisíveis. As plataformas continuam com colisão explícita. As paredes, pisos e fundos ainda são de protótipo; brinquedos, sombras, redemoinhos, Saci e efeitos de folhas precisam de arte final. A galinha usa animações existentes. Não há cutscenes finais, parallax dedicado, escadas/cipós, física de empurrar objetos ou controles de toque Android.

A revisão também tornou os bônus de saúde idempotentes no carregamento da árvore, limitou a redução de cooldown para impedir esquiva invulnerável contínua e restaurou frutas no início de uma partida nova. A importação da animação de morte preserva os 192×192 pixels da folha original, sem redimensionamento NPOT, compressão ou mipmaps.

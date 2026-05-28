# Indaia Fighter — Documentação Técnica

> Gerado em: 2026-05-26  
> Scripts analisados: `Assets/Scripts/Lutador.cs`, `Assets/Scripts/AtaqueHitbox.cs`

---

## 1. Arquitetura Atual

### Estrutura de Prefab de cada Personagem

```
[Personagem Root]
  ├── Rigidbody2D          (Dynamic)
  ├── BoxCollider2D        (corpo físico, NÃO é trigger)
  ├── Animator
  ├── SpriteRenderer
  ├── Lutador.cs
  │
  ├── [SocoHitbox]         (objeto filho)
  │     ├── BoxCollider2D  (IsTrigger = true)
  │     └── AtaqueHitbox.cs
  │
  └── [ChuteHitbox]        (objeto filho)
        ├── BoxCollider2D  (IsTrigger = true)
        └── AtaqueHitbox.cs
```

As hitboxes de ataque são **ativadas e desativadas via keyframes** na janela Animation, durante a animação do golpe.

---

### Personagens existentes

| # | Nome | Prefab |
|---|------|--------|
| 01 | Meyner Jr | `MeynerJr_Prefab.prefab` |
| 02 | Winny Jr | `Winny_Prefab.prefab` |
| 03 | Oroeno | `Oroeno_Prefab.prefab` |
| 04 | Lipe Netão | `LipeNetao_Prefab.prefab` |
| 05 | Luvinha Pedra | `LuvinhaPedra_Prefab.prefab` |
| 06 | Virgi Bodysplash | `Virgi_Prefab.prefab` |
| 08 | Anotta | `Anotta_Prefab.prefab` |
| 09 | Julitt Cacto | `JulittCacto_Prefab.prefab` |
| — | Lu Somsa | `LuSomsa_Prefab.prefab` |

---

### Parâmetros do Animator Controller (por personagem)

| Parâmetro | Tipo | Descrição |
|-----------|------|-----------|
| `IsRun` | Bool | Personagem se movendo horizontalmente |
| `IsJump` | Bool | Personagem no ar |
| `estaAbaixando` | Bool | Agachado |
| `estaBloqueando` | Bool | Bloqueando |
| `Soco` | Trigger | Dispara animação de soco |
| `Chute` | Trigger | Dispara animação de chute |
| `Morte` | Trigger | Dispara animação de morte |
| `Vitoria` | Trigger | Dispara animação de vitória |

---

### Controles

| Ação | Player 1 | Player 2 |
|------|----------|----------|
| Mover esquerda | `A` | `← (LeftArrow)` |
| Mover direita | `D` | `→ (RightArrow)` |
| Pular | `Space` | `↑ (UpArrow)` |
| Agachar | `S` | `PageDown` |
| Bloquear | `LeftShift` | `RightShift` |
| Soco | `F` | `K` |
| Chute | `G` | `L` |

---

### Sistema de Vida

- Vida máxima: `100`
- Bloqueio reduz dano à metade
- UI: Slider `VidaP1` / `VidaP2` (buscados por `GameObject.Find` se não atribuídos no Inspector)

---

## 2. Scripts — Estado Atual (pós-correção)

### `Lutador.cs` — Responsabilidades

| Método | O que faz |
|--------|-----------|
| `Start()` | Inicializa `rb`, `anim`, `sr`, vida, e busca o Slider de HP |
| `Update()` | Chama `InputsJogador()` e `Ataques()` (bloqueado se `morto`) |
| `FixedUpdate()` | Chama `Mover()` (bloqueado se `morto`) |
| `InputsJogador()` | Lê inputs do teclado, atualiza `moveX`, `estaAbaixado`, `estaBloqueando` |
| `Mover()` | Define `rb.velocity.x`, vira o sprite via `localScale.x`, atualiza `IsRun` |
| `Jump()` | Aplica força vertical, ativa `IsJump` |
| `Ataques()` | Dispara triggers `Soco`/`Chute` via input |
| `TakeDamage(int)` | Aplica dano, atualiza slider, chama `Die()` se vida ≤ 0 |
| `PiscarVermelho()` | Coroutine: SpriteRenderer fica vermelho por 0.15s |
| `Die()` | Mata o lutador (ver detalhes abaixo) |
| `DesativarAnimatorAposMorte()` | Coroutine: desliga o Animator após a animação de Morte terminar |
| `ComemorarVitoria()` | Dispara animação de vitória, trava o script |
| `OnCollisionEnter2D()` | Detecta colisão com o chão (`tag = "Ground"`) para resetar `isGrounded` |

#### Fluxo de `Die()` (estado atual corrigido)

```
morto = true
rb.velocity = zero
rb.constraints = FreezeAll          ← trava posição e rotação completamente
colisorPrincipal.enabled = false    ← desativa colisão do corpo
anim.SetBool("IsRun", false)        ← zera todos os bools do Animator
anim.SetTrigger("Morte")            ← dispara animação de morte
StartCoroutine(DesativarAnimatorAposMorte())  ← agenda desligar o Animator
this.enabled = false                ← desativa o script (Update/FixedUpdate param)
```

---

### `AtaqueHitbox.cs` — Responsabilidades

| Método | O que faz |
|--------|-----------|
| `Awake()` | Encontra `Lutador` no objeto pai via `GetComponentInParent` |
| `OnEnable()` | Reseta `jaDeuDano = false` a cada nova ativação da hitbox |
| `OnTriggerEnter2D()` | Aplica dano ao oponente (uma vez por ativação) |

#### Lógica anti-dano-duplicado

```
OnEnable()  →  jaDeuDano = false          (animação ativou a hitbox: novo golpe)
OnTriggerEnter2D()
  se jaDeuDano → return                   (já acertou, ignora)
  se oponente encontrado e é time adversário:
      oponente.TakeDamage(danoDoAtaque)
      jaDeuDano = true                    (trava para este golpe)
      ← NÃO chama SetActive(false) ←     (animação controla a desativação)
```

---

## 3. Bugs Corrigidos

---

### Bug 1 — Dano Múltiplo no Soco (10 → 40 de dano)

**Causa raiz:**

`gameObject.SetActive(false)` era chamado dentro de `OnTriggerEnter2D`. A animação ainda tinha o objeto marcado como ativo nos keyframes seguintes, então **re-ativava a hitbox imediatamente**. Isso disparava `OnEnable` (que faz `jaDeuDano = false`) com o oponente ainda dentro do trigger, causando um segundo `OnTriggerEnter2D`. O ciclo repetia 3-4 vezes por golpe.

```
Antes (errado):
  OnTriggerEnter2D → dano → jaDeuDano=true → SetActive(false)
  Animação re-ativa → OnEnable → jaDeuDano=false → OnTriggerEnter2D → dano novamente ×4

Depois (correto):
  OnTriggerEnter2D → dano → jaDeuDano=true
  (fim — animação desativa a hitbox no keyframe correto)
```

**Correção em `AtaqueHitbox.cs`:**
- Removida a linha `gameObject.SetActive(false)` do `OnTriggerEnter2D`

---

### Bug 2 — Hitbox do P2 falhando

**Causa raiz:**

`GetComponentInParent<Lutador>()` era chamado em `Start()`. Para o P2 instanciado como clone em runtime, pode haver um frame de atraso antes da hierarquia estar montada, resultando em `meuLutador = null`. Com `meuLutador null`, a hitbox nunca causava dano.

**Correção em `AtaqueHitbox.cs`:**
- Mudado de `Start()` para `Awake()` — `Awake` roda antes de qualquer `Start` e antes de qualquer `OnEnable`, garantindo que a referência existe.
- Adicionada guarda `if (meuLutador == null) return;` no `OnTriggerEnter2D`.

**Configuração adicional necessária no Inspector (Layer Collision Matrix):**

Ir em **Edit → Project Settings → Physics 2D → Layer Collision Matrix** e verificar:
- A camada das hitboxes deve estar habilitada para interagir com a camada do corpo dos personagens.
- Se tudo estiver em `Default`, garantir que `Default × Default` está marcado.

---

### Bug 3 — Deslizamento (Chão Liso)

**Causa raiz:**

O Physics Material 2D com **Friction alta** era o problema, não a solução. Quando dois corpos dinâmicos com alta fricção colidem horizontalmente, a física aplica forças de atrito lateral entre eles, criando o efeito de "patinação". Como toda a movimentação horizontal é controlada por script via `rb.velocity`, a fricção física só adiciona forças indesejadas.

**Correção no Inspector (obrigatório — não é no código):**

1. Crie (ou edite) um **Physics Material 2D**:
   - `Friction = 0`
   - `Bounciness = 0`
2. Atribua esse material ao **BoxCollider2D do personagem** (root de cada Prefab).
3. No **Rigidbody2D** de cada personagem, defina:
   - `Linear Drag = 4` — mata impulsos residuais de colisão em ~1 frame

**Correção ideal (Layer Collision Matrix):**

Criar camadas separadas `Player1` e `Player2`, e na Layer Matrix desmarcar a interação entre elas. Assim os corpos físicos dos personagens **não se empurram**, eliminando o deslizamento completamente. As hitboxes (em camada `Hitbox`) continuam interagindo com o corpo do oponente.

| Interação | Deve colidir? |
|-----------|--------------|
| Player × Player | **Não** — sem empurrão físico entre personagens |
| Hitbox × Player | **Sim** — hitbox detecta o corpo do oponente |
| Hitbox × Hitbox | **Não** — hitboxes não interagem entre si |
| Player × Ground | **Sim** |

---

### Bug 4 — Morto Correndo no Chão (IsRun = true após morte)

**Causa raiz (dupla):**

1. `rb.bodyType = RigidbodyType2D.Static` — um corpo Static ainda é movido pelo **Transform**. Se o Animator tiver Root Motion residual ou realizar uma transição de estado indevida, o Transform muda e o corpo "desliza".

2. O **Animator continua ativo após a morte**. Se o estado "Morte" no Animator Controller tiver uma **transição de saída** (ex.: para Idle ou Run), o state machine volta ao fluxo normal após a animação de morte terminar, e `IsRun` pode ser setado verdadeiro por qualquer estado intermediário.

**Correção no código (`Lutador.cs`):**

```csharp
// Antes:
rb.bodyType = RigidbodyType2D.Static;

// Depois:
rb.constraints = RigidbodyConstraints2D.FreezeAll;
```

`FreezeAll` trava posição X, Y e rotação Z diretamente nas constraints físicas, resistindo inclusive a mudanças de Transform — o que Static não faz.

Adicionada Coroutine `DesativarAnimatorAposMorte()` que:
1. Aguarda 1 frame (para o trigger "Morte" ser consumido)
2. Aguarda o restante da animação de morte
3. Desliga o Animator (`anim.enabled = false`), congelando no último frame

> **Nota:** Coroutines sobrevivem ao `this.enabled = false` (só param com `SetActive(false)` no GameObject ou `StopCoroutine`). Por isso a Coroutine é iniciada antes do `this.enabled = false` e continua rodando após o script ser desativado.

**Correção obrigatória no Animator Controller:**

1. Selecione o estado **"Morte"** no Animator de cada personagem.
2. No Inspector do estado:
   - `Loop Time` → **desmarcado** (false)
3. **Remova todas as transições de saída** do estado Morte (clique direito → Delete Transition em cada seta que sai do estado).
4. O estado Morte deve ser **terminal** — sem nenhuma seta saindo.

---

## 4. Checklist de Configuração do Inspector

Use esta lista ao abrir cada Prefab na Unity para verificar se está correto.

### Rigidbody2D (no root de cada personagem)
- [ ] Body Type: **Dynamic**
- [ ] Gravity Scale: `1` (ou o valor desejado)
- [ ] Linear Drag: **`4`**
- [ ] Angular Drag: `0`
- [ ] Constraints: **Freeze Rotation Z** marcado (durante o jogo; FreezeAll é aplicado só na morte via código)
- [ ] Collision Detection: **Continuous** (evita tunneling em golpes rápidos)

### BoxCollider2D principal (no root de cada personagem)
- [ ] Is Trigger: **desmarcado**
- [ ] Material: Physics Material 2D com `Friction = 0`, `Bounciness = 0`

### BoxCollider2D das Hitboxes (filhos SocoHitbox / ChuteHitbox)
- [ ] Is Trigger: **marcado**
- [ ] O objeto deve começar **desativado** (SetActive false no Prefab) — a animação o ativa

### AtaqueHitbox.cs (em cada filho de hitbox)
- [ ] `danoDoAtaque`: `10` para soco, `15` para chute (ou o valor desejado)
- [ ] O campo `meuLutador` é preenchido automaticamente via `Awake()` — não precisa arrastar no Inspector

### Animator Controller (cada personagem)
- [ ] Estado "Morte": Loop Time = **false**, sem transições de saída
- [ ] Estado "Vitoria": Loop Time = **false** (opcional, mas recomendado)
- [ ] Todos os estados de golpe (Soco, Chute) ativam e desativam as hitboxes via **keyframes de GameObject.SetActive** na janela Animation

### Lutador.cs (no root de cada personagem)
- [ ] `isPlayer1`: marcado para P1, desmarcado para P2
- [ ] `nomeLutador`: nome do personagem (ex.: "Meyner Jr")
- [ ] `healthSlider`: pode deixar vazio (o script busca por `GameObject.Find("VidaP1"/"VidaP2")`), ou arrastar o Slider direto

---

## 5. Referência Rápida — Causa × Correção

| Bug | Sintoma | Causa raiz | Correção |
|-----|---------|-----------|----------|
| **1** | Soco dá 40 dano em vez de 10 | `SetActive(false)` no script brigava com os keyframes, ciclando `OnEnable` e resetando `jaDeuDano` | Removido `SetActive(false)` de `AtaqueHitbox.OnTriggerEnter2D` |
| **2** | Hitbox do P2 não registra dano | `GetComponentInParent` em `Start()` podia retornar null para clones instanciados em runtime | Mudado de `Start()` para `Awake()` em `AtaqueHitbox` |
| **3** | Personagens deslizam ao colidir | Physics Material com Friction alta causa forças laterais indesejadas entre corpos dinâmicos | PhysMat: `Friction=0`, `Bounciness=0`; `Linear Drag=4`; separar camadas de colisão |
| **4** | Morto toca animação de corrida | `bodyType=Static` não resistia ao Transform do Animator; state machine transitava de Morte→Idle→Run | `rb.constraints = FreezeAll`; Coroutine desliga Animator; estado Morte sem transições de saída |

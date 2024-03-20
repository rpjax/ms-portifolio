# Guia de Sintaxe da Linguagem WebQL v2

Este guia oferece uma visão detalhada da sintaxe e da semântica da WebQL v2, uma linguagem de consulta poderosa, ideal para interagir com dados na web.

## Conteúdos

[Introdução](#introduction) <br/>
[Consulta Raiz](#query-root) <br/>
[Paginação com Limit e Offset](#paging) <br/>
[Filtragem com Where](#filtering) <br/>
[Ordenação com Order](#ordering) <br/>
[Contexto de Expressões](#expression-context) <br/>
[Exemplos de Consulta](#examples) <br/>

## Introdução <a name="introduction">

WebQL v2 é uma linguagem de consulta robusta e versátil, projetada para facilitar a manipulação e o acesso a dados na web.

## Consulta Raiz <a name="query-root">

A consulta raiz em WebQL v2 pode incluir propriedades especiais com funções definidas:

### Paginação com Limit e Offset <a name="paging">

* limit: Define o número máximo de itens a serem retornados.
* offset: Define a posição inicial para a consulta, usada em conjunto com limit.

### Filtragem com Where <a name="filtering">

* where: Define um escopo de filtragem, permitindo especificar condições detalhadas para os dados que deseja recuperar.

### Ordenação com Order <a name="ordering">

* order: Especifica as propriedades pelas quais os dados devem ser ordenados, suportando múltiplas propriedades no estilo orderby seguido de thenby.

### Conceito de Contexto dentro de Expressões <a name="expression-context">

Em WebQL v2, o contexto dentro de expressões é um aspecto fundamental que define como as expressões são resolvidas e interpretadas. O contexto se refere ao tipo de dado ou à entidade sobre a qual a expressão opera.

#### Contexto Inicial: A Entidade Raiz

O contexto inicial em qualquer consulta WebQL v2 é sempre o tipo da entidade raiz, ou typeof(T).
Este contexto raiz define o escopo inicial de propriedades e métodos disponíveis para a expressão.

#### Nichos de Contexto: Acesso a Membros e Operadores de Iteração

À medida que as expressões acessam membros de um objeto ou entidade, o contexto se ajusta para refletir o tipo do membro acessado.
Operações de acesso a membros mudam o contexto para o tipo da propriedade ou do membro específico.

#### Exemplo de Mudança de Contexto com Acesso a Membros
```json
{
    "propriedade": {
        "subPropriedade": {
            /* O contexto aqui é o tipo de 'subPropriedade' */
        }
    }
}
```

#### Contexto e Operadores de Iteração

Operadores como $any e $all podem alterar o contexto quando usados para iterar sobre propriedades iteráveis (arrays, listas, coleções).
Neste caso, o contexto se torna o tipo dos elementos da coleção.

#### Exemplo de Contexto com Operadores de Iteração
```json
{
    "coleção": {
        "$any": {
            "elemento": "valor"
            /* O contexto aqui é o tipo dos elementos dentro de 'coleção' */
        }
    }
}
```

#### Importância do Contexto

Compreender o contexto é essencial para formular expressões válidas e significativas em WebQL v2.
O contexto guia o usuário sobre quais propriedades e operações são possíveis em cada ponto da consulta.

## Exemplos de Consulta <a name="examples">

A seguir, exemplos variados de consultas em WebQL v2 demonstram a flexibilidade e a potência da linguagem:

### Exemplo 1: Paginação Simples
```json
{
    "limit": 10,
    "offset": 20
}
```
Busca os próximos 10 itens, começando do item 21.

### Exemplo 2: Filtragem Avançada
```json
{
    "where": {
        "categoria": "eletrônicos",
        "preço": {
            "$greater": 100
        }
    }
}
```
Filtra itens na categoria 'eletrônicos' com preço maior que 100.

### Exemplo 3: Ordenação Composta
```json
{
    "order": {
        "dataCriacao": "descending",
        "nome": "ascending"
    }
}
```
Ordena os dados primeiro por 'dataCriacao' em ordem decrescente, e depois por 'nome' em ordem crescente.

### Exemplo 4: Filtragem com Subescopos
```json
{
    "where": {
        "marca": "XYZ",
        "especificacoes": {
            "cor": "azul",
            "tamanho": {
                "$any": ["M", "G"]
            }
        }
    }
}
```
Busca itens da marca 'XYZ' que sejam azuis e nos tamanhos 'M' ou 'G'.

### Exemplo 5: Consulta Complexa com Paginação e Ordenação
```json
{
    "limit": 5,
    "offset": 10,
    "where": {
        "disponibilidade": "emEstoque",
        "avaliacao": {
            "$greaterEquals": 4
        }
    },
    "order": {
        "avaliacao": "descending"
    }
}
```

Busca os próximos 5 itens em estoque com avaliação maior ou igual a 4, começando do 11º item, ordenados por avaliação em ordem decrescente.

# Operadores em WebQL v2

WebQL v2 oferece uma variedade de operadores que podem ser usados para construir expressões complexas em consultas. Aqui está um guia para entender como cada operador funciona:

## Operadores de Comparação Aritmética

Estes operadores são usados com valores numéricos:

* LessThan ($less): Menor que. 
* LessOrEquals ($lessEquals): Menor ou igual a. 
* Greater ($greater): Maior que. 
* GreaterOrEquals ($greaterEquals): Maior ou igual a.

## Operadores de Comparação de Igualdade

Estes operadores podem ser usados com um valor ou uma lista de valores:

* Equals ($equals): Igual a.
* NotEquals ($notEquals): Não igual a.

### Uso com Arrays

Quando usados com arrays, representam várias expressões ligadas por um "OR":

```json
{
    "campo": {
        "$equals": ["valor1", "valor2"] 
    }
}
```

É possível omitir o operador $equals. Quando isso acontece a linguagem infere o uso do operador.

```json
{
    "campo": ["valor1", "valor2"] 
}
```

## Operador Like

Usado exclusivamente com strings:
* Like ($like): Corresponde a um padrão de string.

### Uso com Arrays

Pode ser usado para verificar múltiplos padrões:

```json
{
    "campo": {
        "$like": ["padrão1", "padrão2"] 
    }
}
```

## Operadores Lógicos

* Any ($any): Usado para iterar sobre uma propriedade iterável ou combinar múltiplos escopos de expressão com um "OR".
* All ($all): Semelhante ao $any, mas usado para combinar com um "AND".

Repare que o 'right hand side' de uma expressão de operador '$or' ou '$and' só pode ser um escopo se o contexto for de uma propriedade iterável. Caso contrário apenas um array de escopos será aceito.

Quando usados para iterar uma propriedade, o contexto é automáticamete definido como o tipo contido dentro da coleção iterável.

### Uso com Propriedades Iteráveis, Array de Strings
```json
{
    "listaDeStrings": {
        "$any": [
            { "$like": "foo" },
            { "$like": "bar" },
        ]
    }
}
```

### Uso com Propriedades Iteráveis, Array de Objeto
```json
{
    "propriedadeIterável": {
        "$any": {
            "subcampo": "valor"
        }
    }
}
```

Os operadores lógicos '$or' e '$and' também podem ser usados em qualquer contexto, que não seja de uma propriedade iterável, para juntar conjuntos de escopo de expressões. Respectivamente usando 'OR' e 'AND' lógicos.

### Uso com Vários Escopos

Combina escopos com "OR" ($any) ou "AND" ($all):
```json
{
    "$any": [
        { "campo1": "valor1" },
        { "campo2": "valor2" }
    ]
}
```
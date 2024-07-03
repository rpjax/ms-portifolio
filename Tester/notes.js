
/*
* in this scope the accumulator is a queryable source IQueryable<T>

class T {
    id: int;
    nickname: string;
    surnames: string[];
    email: string?;
}

*/

$filter: {
    /*
    * in this scope the accumulator the queryable element T
    */
    nickname: "John",

    surnames: {
        $contains: "Doe",
        $filter: {
            $equals: "Doe"
        },
        $count: {
            $greater: 0
        }
    },
    $equals: [element.nickname, "John"]
    {
        accumulator: element.surnames,
        $contains: [accumulator, "Doe"] 
        $filter: [accumulator, { $equals: [element, "Doe"] }],
        $count: [accumulator, { $greater: [element, 0] }]
    }

    email: {
        $not: null,
        domain: {
            $equals: "gmail"
        }
    },

    {
        accumulator: accumulator.email,
        $not: [accumulator, null],
        {
            accumulator: element.email.domain,
            $not: [accumulator, null],
        }
    }



    /*
    ## key points:

    ### The Accumulator Symbol.

    # the accumulator is only changed by collection operators.
     
    // # the accumulator is always a queryable source IQueryable<T>.
     
    # the scope defines an evaluation semantic type: aggregation, filtering/logical, projection.
    
        - aggregation: all expressions within the block will be evaluated sequentially, independently, and the last expression will be the result of the block. This result will be stored in the accumulator within the context of the block being evaluated.

        - filtering/logical: all expressions will be evaluated independently and combined with a logical AND.

        - projection: the accumulator doesn't change, and the scope expression will be interpreted as a projection, in a key-value pair format. e.g. $project: { nickname: identity.nickname }
        
    # the initial value of the accumulator, at the root, is the queryable source IQueryable<T>, and can be changed by collection operators. e.g. $filter, $count, $select

        - Collection operators set the accumulator as a declaration for the elements T for all its operands. It also declares all the properties of the type T as local symbols.

        - Scope access expressions get removed from the tree, they are used to create the LHSs of the operators. 
        E.g. $filter: { identity: { nickname: "John" } } -> $filter: { identity.nickname: "John" }

    */
    surnames: {
        $filter: {
            $equals: "Doe"
        }, -> $filter: [surnames, { $equals: [<acc>", Doe"] }]
        $count: {
            $greater: 0
        } -> $count: [surnames, { $greater: [<acc>, 0] }]
    },
    email: {
        $not: null -> $not: [email, null]
    }
},
$select: {
    id: id
}
reagent-effect-guidebook-deal-stamina-damage =
    { $chance ->
        [1]
            { $deltasign ->
                [1] сделки
               *[-1] исцеляет
            }
       *[other]
            { $deltasign ->
                [1] сделка
               *[-1] исцеление
            }
    } { $amount } { $immediate ->
        [true] немедленно
       *[false] сверхурочные
    } урон выносливости

<script setup lang="ts">
import BookCard from "@/components/bibleprogress/BookCard.vue";

const oldTestament = [
  {
    name: "Пятикнижие",
    books: [

      {
        "id": 0,
        "title": "Бытие",
        "chaptersCount": 50
      },
      {
        "id": 1,
        "title": "Исход",
        "chaptersCount": 40
      },
      {
        "id": 2,
        "title": "Левит",
        "chaptersCount": 27
      },
      {
        "id": 3,
        "title": "Числа",
        "chaptersCount": 36
      },
      {
        "id": 4,
        "title": "Второзаконие",
        "chaptersCount": 34
      }

    ]
  },
  {
    "name": "Исторические книги",
    "books": [

      {
        "id": 5,
        "title": "Иисус Навин",
        "chaptersCount": 24
      },
      {
        "id": 6,
        "title": "Судей",
        "chaptersCount": 21
      },
      {
        "id": 7,
        "title": "Руфь",
        "chaptersCount": 4
      },
      {
        "id": 8,
        "title": "1-Царств",
        "chaptersCount": 31
      },
      {
        "id": 9,
        "title": "2-Царств",
        "chaptersCount": 24
      },
      {
        "id": 10,
        "title": "3-Царств",
        "chaptersCount": 22
      },
      {
        "id": 11,
        "title": "4-Царств",
        "chaptersCount": 25
      },
      {
        "id": 12,
        "title": "1-Паралипоменон",
        "chaptersCount": 29
      },
      {
        "id": 13,
        "title": "2-Паралипоменон",
        "chaptersCount": 36
      },
      {
        "id": 14,
        "title": "Ездра",
        "chaptersCount": 10
      },
      {
        "id": 15,
        "title": "Неемия",
        "chaptersCount": 13
      },
      {
        "id": 16,
        "title": "Есфирь",
        "chaptersCount": 10
      }

    ]
  },
  {
    "name": "Поэзия",
    books: [

      {
        "id": 17,
        "title": "Иов",
        "chaptersCount": 42
      },
      {
        "id": 18,
        "title": "Псалтирь",
        "chaptersCount": 150
      },
      {
        "id": 19,
        "title": "Притчи",
        "chaptersCount": 31
      },
      {
        "id": 20,
        "title": "Екклесиаст",
        "chaptersCount": 12
      },
      {
        "id": 21,
        "title": "Песни Песней",
        "chaptersCount": 8
      }

    ]
  },
  {
    "name": "Пророки",
    books: [

      {
        "id": 22,
        "title": "Исаия",
        "chaptersCount": 66
      },
      {
        "id": 23,
        "title": "Иеремия",
        "chaptersCount": 52
      },
      {
        "id": 24,
        "title": "Плач Иеремии",
        "chaptersCount": 5
      },
      {
        "id": 25,
        "title": "Иезекииль",
        "chaptersCount": 48
      },
      {
        "id": 26,
        "title": "Даниил",
        "chaptersCount": 12
      }

    ]
  },
  {
    name: "Малые пророки",
    "books": [

      {
        "id": 27,
        "title": "Осия",
        "chaptersCount": 14
      },
      {
        "id": 28,
        "title": "Иоиль",
        "chaptersCount": 3
      },
      {
        "id": 29,
        "title": "Амос",
        "chaptersCount": 9
      },
      {
        "id": 30,
        "title": "Авдий",
        "chaptersCount": 1
      },
      {
        "id": 31,
        "title": "Иона",
        "chaptersCount": 4
      },
      {
        "id": 32,
        "title": "Михей",
        "chaptersCount": 7
      },
      {
        "id": 33,
        "title": "Наум",
        "chaptersCount": 3
      },
      {
        "id": 34,
        "title": "Аввакум",
        "chaptersCount": 3
      },
      {
        "id": 35,
        "title": "Софония",
        "chaptersCount": 3
      },
      {
        "id": 36,
        "title": "Аггей",
        "chaptersCount": 2
      },
      {
        "id": 37,
        "title": "Захария",
        "chaptersCount": 14
      },
      {
        "id": 38,
        "title": "Малахия",
        "chaptersCount": 4
      }
    ]
  }
]
const newTestament = [
  {
    name: "Евангелия",
    "books": [

      {
        "id": 39,
        "title": "Матфея",
        "chaptersCount": 28
      },
      {
        "id": 40,
        "title": "Марка",
        "chaptersCount": 16
      },
      {
        "id": 41,
        "title": "Луки",
        "chaptersCount": 24
      },
      {
        "id": 42,
        "title": "Иоанна",
        "chaptersCount": 21
      }

    ]
  },
  {
    name: "История Церкви",
    "books": [
      {
        "id": 43,
        "title": "Деяния",
        "chaptersCount": 28
      }
    ]
  },
  {
    name: "Послания",
    "books": [
      {
        "id": 44,
        "title": "Иакова",
        "chaptersCount": 5
      },
      {
        "id": 45,
        "title": "1-Петра",
        "chaptersCount": 5
      },
      {
        "id": 46,
        "title": "2-Петра",
        "chaptersCount": 3
      },
      {
        "id": 47,
        "title": "1-Иоанна",
        "chaptersCount": 5
      },
      {
        "id": 48,
        "title": "2-Иоанна",
        "chaptersCount": 1
      },
      {
        "id": 49,
        "title": "3-Иоанна",
        "chaptersCount": 1
      },
      {
        "id": 50,
        "title": "Иуды",
        "chaptersCount": 1
      },
      {
        "id": 51,
        "title": "Римлянам",
        "chaptersCount": 16
      },
      {
        "id": 52,
        "title": "1-Коринфянам",
        "chaptersCount": 16
      },
      {
        "id": 53,
        "title": "2-Коринфянам",
        "chaptersCount": 13
      },
      {
        "id": 54,
        "title": "Галатам",
        "chaptersCount": 6
      },
      {
        "id": 55,
        "title": "Ефесянам",
        "chaptersCount": 6
      },
      {
        "id": 56,
        "title": "Филиппийцам",
        "chaptersCount": 4
      },
      {
        "id": 57,
        "title": "Колоссянам",
        "chaptersCount": 4
      },
      {
        "id": 58,
        "title": "1-Фессалоникийцам",
        "chaptersCount": 5
      },
      {
        "id": 59,
        "title": "2-Фессалоникийцам",
        "chaptersCount": 3
      },
      {
        "id": 60,
        "title": "1-Тимофею",
        "chaptersCount": 6
      },
      {
        "id": 61,
        "title": "2-Тимофею",
        "chaptersCount": 4
      },
      {
        "id": 62,
        "title": "Титу",
        "chaptersCount": 3
      },
      {
        "id": 63,
        "title": "Филимону",
        "chaptersCount": 1
      },
      {
        "id": 64,
        "title": "Евреям",
        "chaptersCount": 13
      }
    ]
  },
  {
    "name": "Откровения",
    "books": [
      {
        "id": 65,
        "title": "Откровение",
        "chaptersCount": 22
      }
    ]
  }
]
const bible = [{
  name: "Ветхий завет",
  groups: oldTestament
}, {
  name: "Новый завет",
  groups: newTestament
}]
</script>

<template>
  <div class="p-4 bg-gray-50 min-h-screen">
    <div
      v-for="testament in bible"
      :key="testament.name"
      class="mb-8"
    >
      <h1 class="text-xl font-bold text-gray-800 mb-4">
        {{ testament.name }}
      </h1>
      <div
        v-for="group in testament.groups"
        :key="group.name"
        class="mb-6"
      >
        <h2 class="text-lg font-semibold text-gray-700 mb-3">
          {{ group.name }}
        </h2>
        <div class="grid grid-cols-2 md:grid-cols-3 gap-3">
          <BookCard
            v-for="book in group.books"
            :key="book.id"
            :read-chapters="1"
            :total-chapters="book.chaptersCount"
            :book-name="book.title"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>

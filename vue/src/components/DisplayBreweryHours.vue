<template>
  <div>
    <ul class = "brewery-hours">
      <li v-for="displayHours in breweryHours" :key="displayHours.hoursId">
        <span id="day-of-week">{{ displayDayOfWeek(displayHours.dayOfWeek) }}:</span>
        <span v-show="!displayHours.isClosed">
          {{ displayHours.openHour }}:{{
            displayMinutesProperly(displayHours.openMinute)
          }}
          {{ displayHours.openAmPm }} - {{ displayHours.closeHour }}:{{
            displayMinutesProperly(displayHours.closeMinute)
          }}
          {{ displayHours.closeAmPm }}
        </span>
        <span id="closed" v-show="displayHours.isClosed">CLOSED</span>
      </li>
    </ul>
  </div>
</template>

<script>
import api from "../services/apiService.js";
export default {
  data() {
    return {
      breweryHours: [],
    };
  },
  methods: {
    getHours() {
      api.getBreweryHours(this.$route.params.breweryId).then((resp) => {
        this.breweryHours = resp.data;
      });
    },
    displayDayOfWeek(day) {
      if (day === 1) {
        return "Monday";
      } else if (day === 2) {
        return "Tuesday";
      } else if (day === 3) {
        return "Wednesday";
      } else if (day === 4) {
        return "Thursday";
      } else if (day === 5) {
        return "Friday";
      } else if (day === 6) {
        return "Saturday";
      } else {
        return "Sunday";
      }
    },
    displayMinutesProperly(minutes) {
      if (minutes == 0) {
        return "00";
      } else {
        return minutes;
      }
    },
  },
  created() {
    this.getHours();
  },
};
</script>

<style scoped>
li{
  list-style-type: none;
}
ul{
  margin-top: 30px;
}
#closed{
  font-weight: bold;
  color: red;
}
#day-of-week{
  font-weight: bold;
  font-size: 17px;
  margin-right: 5px;
}
</style>
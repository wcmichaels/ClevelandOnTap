<template>
  <div id="container">
    <update-brewery-basic-info class="brewery-info"/>
    <update-hours class="brewery-hours" />

    <span class="update-beer">
      <add-beer />
      <update-beer />
      <display-brewery-beers />
    </span>
    <display-events-one-brewery />
    <new-event-form />
    <delete-event />
    <upload-images />
    <display-brewery-images />
  </div>
</template>

<script>
import UpdateBreweryBasicInfo from "../components/UpdateBreweryBasicInfo.vue";
import UpdateHours from "../components/UpdateBreweryHours.vue";
import UploadImages from "../components/AddBreweryImage.vue";
import AddBeer from "../components/AddBeer.vue";
import UpdateBeer from "../components/UpdateBeer.vue";
import DisplayBreweryBeers from "../components/DisplayBreweryBeers.vue";
import DisplayEventsOneBrewery from "../components/DisplayEventsOneBrewery.vue";
import NewEventForm from "../components/NewEventForm.vue";
import DeleteEvent from "../components/DeleteEvent.vue";
import DisplayBreweryImages from "../components/DisplayBreweryImages.vue"

export default {
  data() {
    return {};
  },
  components: {
    UpdateBreweryBasicInfo,
    UpdateHours,
    UploadImages,
    AddBeer,
    UpdateBeer,
    DisplayBreweryBeers,
    DisplayEventsOneBrewery,
    NewEventForm,
    DeleteEvent,
    DisplayBreweryImages,
  },
};
</script>

<style scoped>
*{
  margin: 1%
}

</style>
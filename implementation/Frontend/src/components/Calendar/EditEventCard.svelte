<script lang="ts">
  import { Button } from "attractions";
  import { SaveIcon, TrashIcon } from "svelte-feather-icons";

  import { makeBackendRequestJson } from "../../lib/requestHelpers";
  import { convertDatesToStrings } from "../../lib/types";
  import {
    deleteEventFromStore,
    editing,
    stopEditing,
    updateEventInStore,
  } from "../../stores/calendar";
  import EventCard from "./EventCard.svelte";

  export let show: boolean;

  async function deleteEvent() {
    const selectedEvent = $editing.selectedEvent;
    if (selectedEvent) {
      const eventDeleted = await makeBackendRequestJson(
        "/Schedule/delete",
        "post",
        { eventId: selectedEvent.id }
      );

      if (eventDeleted) {
        deleteEventFromStore(selectedEvent.id);
        stopEditing();
      }
    }
  }

  async function saveEvent() {
    const selectedEvent = $editing.selectedEvent;
    if (selectedEvent) {
      const isSuccess = await makeBackendRequestJson(
        "/Schedule/edit",
        "put",
        convertDatesToStrings(selectedEvent, ["start", "end"])
      );

      if (isSuccess) {
        updateEventInStore(selectedEvent);
        stopEditing();
      }
    }
  }
</script>

<EventCard
  title="Rediger begivenhed"
  {show}
  event={$editing.selectedEvent}
  on:stopEditing={stopEditing}
>
  <div slot="extra-content" />
  <div slot="buttons" class="flex-row">
    <Button on:click={deleteEvent} danger>
      <TrashIcon />
      <span class="left-margin">Slet</span>
    </Button>
    <Button on:click={saveEvent}>
      <SaveIcon />
      <span class="left-margin">Gem</span>
    </Button>
  </div>
</EventCard>

<style>
  .flex-row {
    display: flex;
    justify-content: space-between;
  }
</style>

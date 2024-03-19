<script lang="ts">
  import {
    Card,
    DatePicker,
    Label,
    Switch,
    TextField,
    TimePicker,
  } from "attractions";
  import { createEventDispatcher } from "svelte";
  import { CalendarIcon } from "svelte-feather-icons";
  import type { WorkEvent } from "../../lib/types";
  import PopupCard from "../PopupCard.svelte";

  export let event:
    | Pick<WorkEvent, "start" | "end" | "note" | "workingFromHome" | "atThePhone">
    | undefined;
  export let show = false;
  export let title: string;

  const dispatch = createEventDispatcher<{
    stopEditing: {};
  }>();
  function correctTimeRange() {
    if (event) {
      event.end.setFullYear(
        event.start.getFullYear(),
        event.start.getMonth(),
        event.start.getDate()
      );

      if (event.start > event.end) {
        event.end = new Date(event.start);
      }
    }
  }
  function stopEditing() {
    dispatch("stopEditing");
  }
</script>

<PopupCard {title} {show} closeCallback={stopEditing}>
  <div slot="title-icon"><CalendarIcon /></div>
  <div class="card-div spaced">
    {#if event}
      <div class="distribute-div">
        <div class="flex-grow">
          <TextField outline withItem label="Note" bind:value={event.note} />
        </div>
      </div>
      <Card style="overflow: visible" outline>
        <div>
          <Label style="display: block">Tid</Label>
          <DatePicker bind:value={event.start} on:change={correctTimeRange} />
          <span>fra</span>
          <TimePicker bind:value={event.start} on:change={correctTimeRange} />
          <span>til</span>
          <TimePicker bind:value={event.end} on:change={correctTimeRange} />
        </div>
      </Card>

      <Card outline>
        <div class="spaced">
          <div class="distribute-div">
            <Label class="attribute-label">Tilgængelig ved telefon</Label>
            <Switch bind:value={event.atThePhone} />
          </div>
          <div class="distribute-div">
            <Label class="attribute-label">Arbejder hjemmefra</Label>
            <Switch bind:value={event.workingFromHome} />
          </div>
          <slot name="extra-options" />
        </div>
      </Card>

      <slot name="extra-content" />

      <slot name="buttons" />
    {:else}
      <Label class="attribute-label">Der er ikke valgt nogen begivenhed</Label>
    {/if}
  </div>
</PopupCard>

<style>
  .card-div {
    display: flex;
    flex-direction: column;
  }

  .distribute-div {
    display: flex;
    justify-content: space-between;
    align-items: center;
    gap: 0.5cm;
  }

  .flex-grow {
    flex-grow: 1;
  }
</style>

<script lang="ts">
  import { startOfDay } from "date-fns";
  import {
    convertTimeToPercent,
    dateFormatter,
    timeFormatter,
  } from "../../lib/calendar";
  import type { WorkEvent } from "../../lib/types";
  import { associates } from "../../stores/associate";
  import { startEditing, vacationsInEventRange } from "../../stores/calendar";
  import NarrowEventContent from "./NarrowEventContent.svelte";
  import WideEventContent from "./WideEventContent.svelte";

  export let event: WorkEvent;
  export let selected: boolean;

  export let columnCount: number = 1;
  export let columnIndex: number = 1;
  export let columnSpan: number = 1;

  $: columnWidth = 100 / columnCount;
  $: width = columnWidth * columnSpan;
  $: left = columnWidth * columnIndex;
  $: top = convertTimeToPercent(event.start) * 100;
  $: bottom = 100 - convertTimeToPercent(event.end) * 100;

  $: isInVacationPeriod = $vacationsInEventRange.some(
    (v) =>
      (v.scheduleId === event.scheduleId ||
        !$associates.some((a) => a.associateScheduleId == v.scheduleId)) &&
      startOfDay(v.start) <= startOfDay(event.start) &&
      startOfDay(v.end) >= startOfDay(event.start)
  );
</script>

<div
  style:top={`${top}%`}
  style:width={`${width}%`}
  style:bottom={`${bottom}%`}
  style:left={`${left}%`}
  style:background-color={$associates.find(
    (a) => a.associateScheduleId === event.scheduleId
  )?.color}
  style:opacity={isInVacationPeriod ? 0.5 : 1}
  title={isInVacationPeriod
    ? "Ferie"
    : `${dateFormatter(event.start)} klokken ${timeFormatter(event.start)}`}
  class="dynamic-event rbc-event"
  class:selected
  role="button"
  tabindex="0"
  on:mouseup={() => {
    startEditing(event);
  }}
>
  {#if width < 50}
    <NarrowEventContent {event} />
  {:else}
    <WideEventContent {event} />
  {/if}
</div>

<style lang="scss">
  @use "../../variables.scss" as variables;
  .rbc-event {
    transition: all 0.25s ease;
    border: none;
    box-sizing: border-box;
    box-shadow: none;
    margin: 0;
    padding: 2px 5px;
    background-color: variables.$event-color;
    border-radius: variables.$small-border-radius;
    color: #fff;
    cursor: pointer;
    width: 100%;
    text-align: left;
  }

  .rbc-event:focus,
  .rbc-event.selected {
    outline: 5px auto variables.$selected-outline-color;
    background-color: variables.$selected-color;
  }

  /* Custom CSS */
  .dynamic-event {
    bottom: 0;
    left: 0;
    position: absolute;
    right: 0;
    margin-right: 10px;
    top: 0;
  }
</style>

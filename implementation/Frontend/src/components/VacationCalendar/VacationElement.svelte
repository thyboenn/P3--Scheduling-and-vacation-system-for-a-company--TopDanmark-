<script lang="ts">
  import { intervalToDuration, isSameDay, startOfMonth } from "date-fns";
  import { CheckIcon, HelpCircleIcon, XIcon } from "svelte-feather-icons";
  import type { Vacation } from "../../lib/types";
  import {
    currentDate,
    editing,
    startEditingVacation,
  } from "../../stores/vacation";

  export let vacation: Vacation;
  export let color: string | null;

  function calculateColspan(vacation: Vacation) {
    if (startOfMonth(vacation.start) < startOfMonth($currentDate)) {
      return vacation.end.getDate() + 1;
    }
    return (
      (intervalToDuration({ start: vacation.start, end: vacation.end }).days ?? 0) +
      1
    );
  }
</script>

<!-- svelte-ignore a11y-click-events-have-key-events -->
<td
  class="vacation"
  style:background-color={color}
  class:selected={$editing.vacationId != undefined &&
    $editing.vacationId == vacation.id &&
    $editing.editing}
  colspan={calculateColspan(vacation)}
  on:click={() => {
    startEditingVacation(vacation.id);
  }}
>
  <div class="vertical-align-center row space-between color-white">
    <div>{vacation.start.getDate()}</div>
    <div>
      {#if vacation.status == "Approved"}
        <CheckIcon />
      {:else if vacation.status == "Denied"}
        <XIcon />
      {:else if vacation.status == "Pending"}
        <HelpCircleIcon />
      {/if}
    </div>
    {#if !isSameDay(vacation.start, vacation.end)}
      <div>{vacation.end.getDate()}</div>
    {/if}
  </div>
</td>

<style lang="scss">
  @use "../../variables.scss" as variables;

  .vacation {
    border-radius: variables.$small-border-radius;
    background-color: variables.$event-color;
    cursor: pointer;
  }

  .vacation:focus,
  .vacation.selected {
    outline: 5px auto variables.$selected-outline-color;
    background-color: variables.$selected-color;
  }

  .color-white {
    color: white;
  }
</style>

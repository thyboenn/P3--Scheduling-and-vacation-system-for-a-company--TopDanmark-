<script lang="ts">
  import { Button } from "attractions";
  import { add, getWeek, sub } from "date-fns";
  import { ChevronLeftIcon, ChevronRightIcon } from "svelte-feather-icons";
  import { daysPerWeek } from "../../lib/calendar";
  import { currentDate } from "../../stores/calendar";
</script>

<div class="flex-column">
  <div class="rbc-toolbar">
    <Button
      round
      on:click={() => currentDate.set(sub($currentDate, { days: daysPerWeek }))}
      type="button"
    >
      <ChevronLeftIcon />
    </Button>
    <div class="rbc-btn-group">
      <Button on:click={() => currentDate.set(new Date())} type="button">
        I dag
      </Button>
      <span class="rbc-toolbar-label">Uge {getWeek($currentDate)}</span>
    </div>
    <Button
      round
      on:click={() => currentDate.set(add($currentDate, { days: daysPerWeek }))}
      type="button"
    >
      <ChevronRightIcon />
    </Button>
  </div>
</div>

<style lang="scss">
  @use "../../variables.scss" as variables;

  .flex-column {
    display: flex;
    flex-direction: column;
    height: 100%;
    justify-content: center;
  }

  .rbc-toolbar {
    display: flex;
    flex-wrap: wrap;
    justify-content: space-between;
    align-items: center;
    font-size: 16px;
    padding: 0.25cm;
  }

  .rbc-toolbar .rbc-toolbar-label {
    padding: 0 10px;
    text-align: center;
  }

  .rbc-btn-group {
    display: flex;
    align-items: center;
    white-space: nowrap;
  }
</style>

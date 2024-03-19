<script lang="ts">
  import { dateFormatter, getWeekDays } from "../../lib/calendar";
  import { currentDate } from "../../stores/calendar";
</script>

<div class="rbc-time-header-content">
  <div class="rbc-row rbc-time-header-cell">
    {#each getWeekDays($currentDate) as day}
      <div class="rbc-header">
        {dateFormatter(day)}
      </div>
    {/each}
  </div>
</div>

<style lang="scss">
  @use "../../variables.scss" as variables;

  .rbc-time-header-content {
    flex: 1;
    display: flex;
    min-width: 0;
    flex-direction: column;
    border-left: variables.$border-style;
    height: 100%;
    border-bottom: variables.$border-style;
  }

  .rbc-row {
    display: flex;
    flex-direction: row;
    height: 100%;
  }

  .rbc-header {
    overflow: hidden;
    flex: 1 0 0%;
    display: flex;
    justify-content: center;
    align-items: center;
    text-overflow: ellipsis;
    white-space: nowrap;
    padding: 0 3px;
    font-weight: bold;
    font-size: 90%;
    min-height: 0;
    width: 0.1cm;
  }

  .rbc-header + .rbc-header {
    border-left: 1px solid variables.$border-color;
  }
</style>

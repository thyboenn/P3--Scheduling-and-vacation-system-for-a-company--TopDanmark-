<script>
  import { hourStrings } from "../../lib/calendar";
</script>

<div class="rbc-time-column">
  {#each hourStrings as hour}
    <div class="rbc-timeslot-group">
      <div class="rbc-label">{hour.text}</div>
    </div>
  {/each}
</div>

<style>
  .rbc-time-column {
    min-height: 100%;
  }
  .rbc-label {
    padding: 0 5px;
  }
</style>

<script lang="ts">
  import { associates, updateAssociates } from "../../stores/associate";
  import {
    currentDate,
    hiddenVacations,
    updateVacations,
    vacations,
  } from "../../stores/vacation";
  import DaysRow from "./DaysRow.svelte";
  import VacationRow from "./VacationRow.svelte";

  currentDate.subscribe(async (newDate) => {
    updateAssociates();
    updateVacations(newDate);
  });
</script>

<div class="scroll">
  <table class="full-width">
    <tr>
      <td />
      <DaysRow />
    </tr>
    <VacationRow
      vacations={$vacations.filter(
        (vacation) =>
          !$associates.some((a) => a.associateScheduleId === vacation.scheduleId)
      )}
      name="Fælles"
    />
    {#each $associates as associate}
      {#if associate.associateScheduleId && !$hiddenVacations.includes(associate.associateScheduleId)}
        <tr>
          <VacationRow
            vacations={$vacations
              .filter(
                (vacation) => vacation.scheduleId == associate.associateScheduleId
              )
              .sort((a, b) => Number(a.start) - Number(b.start))}
            name={associate.name}
            color={associate.color}
          />
        </tr>
      {/if}
    {/each}
  </table>
</div>

<style lang="scss">
  @use "../../variables.scss" as variables;
  .scroll {
    min-height: 100vh;
    overflow: scroll;
  }
  .full-width {
    width: 100%;
  }
  table {
    border-collapse: collapse;
  }
  tr {
    border-bottom: variables.$border-style;
  }
  td {
    border-right: variables.$border-style;
  }
</style>

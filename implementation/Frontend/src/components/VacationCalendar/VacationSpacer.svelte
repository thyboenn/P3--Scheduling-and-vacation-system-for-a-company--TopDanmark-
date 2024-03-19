<script lang="ts">
  export let colspan: number;
</script>

{#if colspan > 0}
  <td {colspan} />
{/if}
